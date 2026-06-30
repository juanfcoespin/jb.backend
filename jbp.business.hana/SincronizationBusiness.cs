using jbp.core.sapDiApi;
using jbp.msg;
using jbp.msg.sap;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTools.Core.Hana;
using static jbp.business.hana.SincronizationBusiness;

namespace jbp.business.hana
{
    public class SincronizationBusiness:BaseBusiness
    {
        public delegate void dDocsToSync(List<DocsToSyncMsg> docs);
        public event dDocsToSync onDocsToSync;
        private object _lockSincPedidosYCobros = new object();

        public delegate void dNotyfySyncStatus(DocsToSyncMsg doc);
        public event dNotyfySyncStatus onChangeSyncStatus;
        public event dNotyfySyncStatus onDocSyncOK;

        public void SincronizarPedidoYCobros()
        {
            lock (_lockSincPedidosYCobros)
            {
                try
                {
                    var pedidos = GetDocsVetToSyncByTipo(eTipoDocToSync.Pedido);
                    if (pedidos != null && pedidos.Count > 0)
                    {
                        onDocsToSync?.Invoke(pedidos);
                        SyncDocs(pedidos, eTipoDocToSync.Pedido);
                    }
                    var cobros = GetDocsVetToSyncByTipo(eTipoDocToSync.Cobro);
                    if (cobros != null && cobros.Count > 0)
                    {
                        onDocsToSync?.Invoke(cobros);
                        SyncDocs(cobros, eTipoDocToSync.Cobro);
                    }

                    //reset a los q se quedaron en procesando 1
                    resetEstadoProcesando();
                }
                catch (Exception e)
                {
                    RaiseError(e.Message + e.StackTrace);
                }
            }
        }

        private void resetEstadoProcesando()
        {
            // pone en estado no procesado 0 si no se proceso correctamente 1
            try
            {
                var sql = string.Format(@"
                update JB_CACHE_DOCS_VET_TO_SYNC        
                set PROCESANDO=0
                where PROCESANDO = ?
            ");
                new BaseCore().Execute(sql, new Dictionary<string, object> {
                    {"@0", 1}
                });
            }
            catch (Exception e)
            {
                //Rollback();
                throw new Exception(
                    "Error al reiniciar el estado a no procesado",
                    e
                );
            }
        }

        private bool IsAlreadySynchronized(object idCache){
            try{
                var sql = "select count(*) from JB_HISTORICO_DOCS_SINCRONIZADOS where ID = ?";
                var count = new BaseCore().GetIntScalarByQuery(sql, new Dictionary<string, object> { { "@0", idCache } });
                return count > 0;
            }
            catch { return false; }
        }

        private void SyncDocs(List<DocsToSyncMsg> docsToSync, eTipoDocToSync tipoDocToSync)
        {
            foreach (var docToSync in docsToSync.ToList())
            {
                var currentDoc = docToSync;

                // Consulta en histórico para comprobar si ya se sincronizó y evitar registros repetidos en SAP
                if (IsAlreadySynchronized(currentDoc.IdCache)){
                    var errMsg = "El documento ya existe en el histórico. Se asume repetido. No se sincronizará nuevamente.";
                    NofifySyncStatus(currentDoc, errMsg, eTipoMsg.Error);
                    currentDoc.Error = errMsg;
                    RegistrarErrEnCache(currentDoc);
                    NotificarErrorPorCorreo(currentDoc, errMsg);
                    continue; // Saltar este documento
                }

                BaseSapObj sapDiapiObj = null;
                if (tipoDocToSync == eTipoDocToSync.Pedido)
                    sapDiapiObj = new jbp.core.sapDiApi.SapOrder();
                if (tipoDocToSync == eTipoDocToSync.Cobro)
                    sapDiapiObj = new jbp.core.sapDiApi.SapPagoRecibido();
                if (sapDiapiObj == null)
                    throw new Exception("No se ha podido determinar el objeto DIAPI!!");

                BaseSapObj.dNotififacationMessage handler = null;
                handler = (msg) =>
                {
                    try
                    {
                        NofifySyncStatus(currentDoc, msg);
                    }
                    catch(Exception e) {
                        TechTools.Utils.Logger.Error("handler - bussiness");
                        TechTools.Utils.Logger.Error(e);
                    }
                };
                try
                {
                    sapDiapiObj.onNotififacationMessage += handler;
                    if (!sapDiapiObj.Connect())
                        throw new Exception("No se pudo conectar a SAP");

                    var resp = "";
                    if (tipoDocToSync == eTipoDocToSync.Pedido)
                    {
                        /*
                        ----- Asignar precios a lineas de pedido -------
                        - Del app ya viene calculado el descuento financiero y la bonificación
                        - Desde sap se requiere poner otra vez la lógica  x esto se setea denuevo el precio

                        - se crea una copia del objeto para registrar en el historico del cache tal cual el precio le
                            apareció al vededor
                        */
                        var pedido = (OrdenMsg)((OrdenMsg)docToSync).Clone();
                        pedido.Lines.ForEach(line =>
                        {
                            line.price = SocioNegocioBusiness.GetPrecioByCodSocioNegocioCodArticulo(pedido.CodCliente, line.CodArticulo);
                        });

                        resp = ((SapOrder)sapDiapiObj).Add(pedido);
                    }

                    if (tipoDocToSync == eTipoDocToSync.Cobro)
                        resp = ((SapPagoRecibido)sapDiapiObj).SafePago((PagosMsg)docToSync);
                    if (resp == "ok")
                    {
                        NofifySyncStatus(docToSync, tipoDocToSync.ToString() + " registrado en SAP correctamente!!");
                        MoveToHistorico(docToSync);
                        // para mostrar en capa de presentación, en bdd ya se guarda al mover al historico
                        docToSync.FechaIngresoSap = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        try
                        {
                            onDocSyncOK?.Invoke(docToSync);
                        }
                        catch (Exception ex) {
                            TechTools.Utils.Logger.Error("onDocSyncOK - business");
                            TechTools.Utils.Logger.Error(ex);
                        }
                    }
                    else //se notifica el error
                    {
                        NofifySyncStatus(docToSync, "Registrando error en bdd");
                        currentDoc.Error = resp;
                        RegistrarErrEnCache(currentDoc);
                        NotificarErrorPorCorreo(currentDoc, "Error en sincronización de doc VET");
                        NofifySyncStatus(currentDoc, resp, eTipoMsg.Error);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally {
                    try
                    {
                        sapDiapiObj.onNotififacationMessage -= handler;
                    }
                    catch (Exception e) {
                        TechTools.Utils.Logger.Error("Bussines: quitar handlers");
                        TechTools.Utils.Logger.Error(e);
                    }
                    try
                    {
                        sapDiapiObj.Disconnect();
                    }
                    catch (Exception e)
                    {
                        TechTools.Utils.Logger.Error("Bussines: Desconexión sap");
                        TechTools.Utils.Logger.Error(e);
                    }
                    
                    
                }                     
            }            
        }

        

        private static void ActualizarEstado(string id, string estado)
        {
            var sql = string.Format(@"
                update JB_CACHE_DOCS_VET_TO_SYNC        
                set PROCESANDO=?
                where ID=?
            ");
            new BaseCore().Execute(sql, new Dictionary<string, object> {
                { "@0", estado},
                { "@1", id},
            });
        }
        private OrdenMsg GetOrderdenMsgFromStrJson(string strJson)
        {
            var order = TechTools.Serializador.SerializadorJson.Deserializar(typeof(OrdenMsg), strJson);
            var ms = (OrdenMsg)order;
            ms.Lines.ForEach(line => {
                if (string.IsNullOrEmpty(line.Articulo) && !string.IsNullOrEmpty(line.CodArticulo))
                    line.Articulo = ProductBusiness.GetNombreArticuloByCodigo(line.CodArticulo);
                if (line.price > 0)
                    line.price = Math.Round(line.price, 2);
            });
            return ms;
        }
        private PagosMsg GetCobroMsgFromStrJson(string strJson)
        {
            var cobro = TechTools.Serializador.SerializadorJson.Deserializar(typeof(PagosMsg), strJson);
            var ms = (PagosMsg)cobro;
            //para que no se procesen las fotos en la sincronización con sap
            //este proceso se hace antes
            ms.fotosComprobantes = null;
            return ms;
        }
        private void RegistrarErrEnCache(DocsToSyncMsg docToSync)
        {
            if (docToSync != null && docToSync.IdCache !=null) {
                try
                {
                    var sql = string.Format(@"
                        update JB_CACHE_DOCS_VET_TO_SYNC
                        set ERROR=?,
                        PROCESANDO=0,
                        NUM_INTENTOS = NUM_INTENTOS +1,
                        FECHA_ULTIMO_INTENTO = CURRENT_TIMESTAMP
                        where ID=?
                    ");
                    new BaseCore().Execute(sql, new Dictionary<string, object> {
                        {"@0", docToSync.Error },
                        {"@1", docToSync.IdCache },
                    });
                }
                catch (Exception e)
                {
                    NofifySyncStatus(docToSync,e.Message + e.StackTrace,eTipoMsg.Error);
                }
            }

            
        }
        private void NotificarErrorPorCorreo(DocsToSyncMsg doc, string titulo = ""){
            var msg = string.Format(@"
                <h1>{7}</h1>
                <div>
                    <p><b>Id:</b> {0}</p>
                    <p><b>Tipo:</b> {1}</p>
                    <p><b>Fecha Sincronización Vendedor:</b> {2}</p>
                    <p><b>Cliente:</b> {3}</p>
                    <p><b>Vendedor:</b> {4}</p>
                    <p><b>Monto:</b> {5}</p>
                    <p><b>Error:<br></b> {6}</p>
                </div>",
                    doc.IdCache,
                    doc.TipoDocumento,
                    doc.FechaSincronizacionVendedor,
                    doc.Cliente,
                    doc.Vendedor,
                    doc.Total,
                    doc.Error,
                    titulo);
            string error = null;
            this.EnviarPorCorreo(conf.Default.correoErrorSync, titulo, msg, ref error);
        }
        //private DocsToSyncMsg docToSyncActual;
        internal void MoveToHistorico(DocsToSyncMsg me)
        {
            try
            {
                NofifySyncStatus(me, "Moviendo pedido a histórico...");
                var strMsg = TechTools.Serializador.SerializadorJson.Serializar(me);
                string strTotal = string.Empty;
                if (me.Total != null) {
                    strTotal = me.Total.ToString();
                    strTotal = strTotal.Replace(",", ".");
                }
                
                // la fecha de ingreso a sap se registra automáticamente
                var sql = string.Format(@"
                    insert into JB_HISTORICO_DOCS_SINCRONIZADOS(ID, TIPO_DOC, FECHA_SINCRONIZACION, VENDEDOR, CLIENTE, MONTO, MSG)
                    values (?,?,?,?,?,?,?)
                ");
                new BaseCore().Execute(sql, new Dictionary<string, object> {
                    {"@0", me.IdCache },
                    {"@1",me.TipoDocumento },
                    {"@2",me.FechaSincronizacionVendedor },
                    {"@3",me.Vendedor },
                    {"@4",me.Cliente },
                    {"@5",strTotal },
                    {"@6",strMsg }
                });
                // se borra el pedido sincronizado de cache
                NofifySyncStatus(me, "Eliminando pedido del caché...");
                sql = string.Format(@"
                    delete from JB_CACHE_DOCS_VET_TO_SYNC where ID=?
                ");
                new BaseCore().Execute(sql, new Dictionary<string, object> {
                    {"@0", me.IdCache} 
                });
            }
            catch (Exception e)
            {
                var err = e.Message + e.StackTrace;
                RaiseError(err);
            }
        }
        private void NofifySyncStatus(DocsToSyncMsg docToSync, string msg, eTipoMsg tipoMsg=eTipoMsg.Info)
        {
            lock (docToSync) {
                // para que no se dupliquen los mensajes
                if (docToSync.MensajesSincronizacion.Exists(ms => ms.Msg == msg))
                    return;
                //concatenar con el status anterior
                docToSync.MensajesSincronizacion.Add(new MsgSincronizacion
                {
                    FechaLog = DateTime.Now.ToString(),
                    Msg = msg,
                    TipoMsg = tipoMsg,
                });
                try
                {
                    onChangeSyncStatus?.Invoke(docToSync);
                }
                catch (Exception e) {
                    TechTools.Utils.Logger.Error("onChangeSyncStatus - business");
                    TechTools.Utils.Logger.Error(e);
                }
                
            }
            
        }
        public List<DocsToSyncMsg> GetDocsVetToSyncByTipo(eTipoDocToSync tipoDocToSync)
        {
            var ms = new List<DocsToSyncMsg>();
            try
            {
                var sql = string.Format(@"
                    SELECT
                     TOP 10   
                     ID,
                     TIPO_DOC,   
                     CLIENTE,
                     VENDEDOR,
                     to_char(FECHA_SINCRONIZACION, 'YYYY-MM-DD HH24:MI:SS') ""FECHA_SINCRONIZACION"",
                     MSG FROM JB_CACHE_DOCS_VET_TO_SYNC
                    where 
                      PROCESANDO=0
                      and TIPO_DOC=?
                      and NUM_INTENTOS<3
                    order by FECHA_SINCRONIZACION
                ");
                var bc = new BaseCore();
                var dt = bc.GetDataTableByQuery(sql, new Dictionary<string, object> {
                    {"@0",tipoDocToSync.ToString() }
                });
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        var docToSync = GetDocToSyncByDataRow(dr);
                        ActualizarEstado(docToSync.IdCache, "1");
                        ms.Add(docToSync);
                    }
                }
            }
            catch (Exception e)
            {
                RaiseError(e.Message + e.StackTrace);
            }
            return ms;
        }

        private DocsToSyncMsg GetDocToSyncByDataRow(DataRow dr)
        {
            DocsToSyncMsg ms = null;
            string error=string.Empty;
            string fechaIngresoSap = string.Empty;
            var idCache = dr["ID"].ToString();
            var tipoDocumento = dr["TIPO_DOC"].ToString();
            var cliente = dr["CLIENTE"].ToString();
            var vendedor = dr["VENDEDOR"].ToString();
            var msgJsonObj = dr["MSG"].ToString();
            try
            {
                error= dr["ERROR"].ToString();
            }
            catch { }
            try
            {
                fechaIngresoSap = dr["FECHA_INGRESO_SAP"].ToString();
            }
            catch { }
            var fechaSincronizacionVendedor = dr["FECHA_SINCRONIZACION"].ToString();
            var eTipoDocumento = eTipoDocToSync.NoDefinido;
            if (tipoDocumento == eTipoDocToSync.Pedido.ToString())
                eTipoDocumento = eTipoDocToSync.Pedido;
            if (tipoDocumento == eTipoDocToSync.Cobro.ToString())
                eTipoDocumento = eTipoDocToSync.Cobro;
            if (eTipoDocumento == eTipoDocToSync.Pedido)
            {
                ms = GetOrderdenMsgFromStrJson(msgJsonObj);
            }
            if (eTipoDocumento == eTipoDocToSync.Cobro)
            {
                ms = GetCobroMsgFromStrJson(msgJsonObj);
            }
            if (ms!=null)
            {
                
                ms.IdCache = idCache;
                ms.FechaSincronizacionVendedor = fechaSincronizacionVendedor;
                ms.Cliente = cliente;
                ms.Vendedor = vendedor;
                ms.MensajesSincronizacion = new List<MsgSincronizacion>();
                ms.Error = error;
                ms.FechaIngresoSap = fechaIngresoSap;
            }
            return ms;
        }

        public List<DocsToSyncMsg> ConsultarHistoricoDocsSincronizados(FiltroHistoricoMsg filtro, eTipoDocToSync tipoDoc)
        {
            var ms = new List<DocsToSyncMsg>();
            try
            {
                
                var desde = filtro.Desde.ToString("yyyy-MM-dd");
                var hasta = filtro.Hasta.ToString("yyyy-MM-dd");
                var sql = string.Format(@"
                    select
                     ID,
                     TIPO_DOC,
                     to_char(FECHA_SINCRONIZACION,'yyyy-mm-dd hh24:mi:ss') FECHA_SINCRONIZACION,
                     to_char(FECHA_INGRESO_SAP,'yyyy-mm-dd hh24:mi:ss') FECHA_INGRESO_SAP,
                     VENDEDOR,
                     CLIENTE,
                     MONTO,
                     FECHA_INGRESO_SAP,
                     MSG
                    from
                     JB_HISTORICO_DOCS_SINCRONIZADOS  
                    where
                     upper(VENDEDOR) like ?
                     and upper(CLIENTE) like ?
                     and FECHA_SINCRONIZACION >= TO_DATE(?,'yyyy-mm-dd')
					 AND FECHA_SINCRONIZACION <  ADD_DAYS(TO_DATE(?,'yyyy-mm-dd'),1)
                ");
                if (tipoDoc != eTipoDocToSync.NoDefinido) {
                    sql += " and upper(TIPO_DOC) like ?";
                }
                sql += " order by FECHA_SINCRONIZACION desc";
                var dt = new BaseCore().GetDataTableByQuery(sql, new Dictionary<string, object> {
                    {"@0", "%"+filtro.Vendedor.ToUpper()+"%" },
                    {"@1","%"+filtro.Cliente.ToUpper()+"%" },
                    {"@2",desde },
                    {"@3", hasta },
                    {"@4","%"+tipoDoc.ToString().ToUpper()+"%" }
                });
                if (dt.Rows != null && dt.Rows.Count > 0) {
                    foreach (DataRow dr in dt.Rows)
                    {
                        var docToSync = GetDocToSyncByDataRow(dr);
                        if (docToSync != null) {
                            ms.Add(docToSync);
                        }
                    }
                }
            }
            catch (Exception e) { 
                RaiseError(e.Message+e.StackTrace);
            }
            return ms;
        }
        internal static string SaveDocEnCache(DocsToSyncMsg me, eTipoDocToSync tipoDoc)
        {
            try
            {
                if (string.IsNullOrEmpty(me.Cliente) && !string.IsNullOrEmpty(me.CodCliente))
                    me.Cliente = SocioNegocioBusiness.GetByCodigo(me.CodCliente);
                if (string.IsNullOrEmpty(me.Vendedor) && !string.IsNullOrEmpty(me.CodCliente))
                    me.Vendedor = SocioNegocioBusiness.GetVendedorByCodSocioNegocio(me.CodCliente).Vendedor;
                var sql = string.Format(@"
                    insert into JB_CACHE_DOCS_VET_TO_SYNC(TIPO_DOC, VENDEDOR, CLIENTE, MONTO, MSG)
                    values (?, ?, ?, ?, ?)
                ");
                new BaseCore().Execute(sql, new Dictionary<string, object> {
                    {"@0", tipoDoc.ToString()},
                    {"@1", me.Vendedor },
                    {"@2", me.Cliente },
                    {"@3", me.Total },
                    {"@4", me.JsonObj }
                });
                return "ok";
            }
            catch (Exception e)
            {
                var err = e.Message;
                return err + e.StackTrace;
            }
        }

        public List<DocsToSyncMsg> ConsultarDocsConError()
        {
            var ms = new List<DocsToSyncMsg>();
            try
            {
                var sql = string.Format(@"
                SELECT 
                 ID,
                 TIPO_DOC,
                 FECHA_SINCRONIZACION,
                 VENDEDOR,
                 CLIENTE,
                 MONTO,
                 MSG,
                 ERROR
                FROM JB_CACHE_DOCS_VET_TO_SYNC
                where ERROR IS NOT null;
                ");
                var dt = new BaseCore().GetDataTableByQuery(sql, null);
                if (dt != null && dt.Rows.Count > 0) {
                    foreach (DataRow dr in dt.Rows)
                    {
                        var docToSync = GetDocToSyncByDataRow(dr);
                        ms.Add(docToSync);
                    }
                }
            }
            catch(Exception e) {
                RaiseError(e.Message+e.StackTrace);
            }
            return ms;
        }
    }
}
