using jbp.core.sapDiApi;
using jbp.msg;
using jbp.msg.sap;
using System;
using System.Collections.Generic;
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

        public delegate void dNotyfySyncStatus(DocsToSyncMsg doc);
        public event dNotyfySyncStatus onChangeSyncStatus;
        public event dNotyfySyncStatus onDocSyncOK;

        public void SincronizarPedidoYCobros()
        {
            try
            {
                var pedidos = GetDocsVetToSyncByTipo(eTipoDocToSync.Pedido);
                if (pedidos != null && pedidos.Count > 0)
                {
                    this.docToSyncActual = pedidos[0];
                    onDocsToSync?.Invoke(pedidos);
                    SyncDocs(pedidos, eTipoDocToSync.Pedido);
                }
                var cobros = GetDocsVetToSyncByTipo(eTipoDocToSync.Cobro);
                if (cobros != null && cobros.Count > 0) {
                    onDocsToSync?.Invoke(cobros);
                    SyncDocs(cobros, eTipoDocToSync.Cobro);
                }
            }
            catch (Exception e) {
                RaiseError(e.Message + e.StackTrace);
            }
            
        }
        private void SyncDocs(List<DocsToSyncMsg> docsToSync, eTipoDocToSync tipoDocToSync)
        {
            BaseSapObj sapDiapiObj =null;
            if(tipoDocToSync==eTipoDocToSync.Pedido)
                sapDiapiObj = new jbp.core.sapDiApi.SapOrder();
            if (tipoDocToSync == eTipoDocToSync.Cobro)
                sapDiapiObj = new jbp.core.sapDiApi.SapPagoRecibido();
            if (sapDiapiObj == null)
                throw new Exception("No se ha podido determinar el objeto DIAPI!!");
            this.docToSyncActual = docsToSync[0];
            sapDiapiObj.onNotififacationMessage += (msg) => {
                NofifySyncStatus(this.docToSyncActual, msg);
            };
            if (sapDiapiObj.Connect())
            {
                try
                {
                    foreach (var docToSync in docsToSync.ToList())
                    {
                        this.docToSyncActual = docToSync;
                        var resp = "";
                        if (tipoDocToSync == eTipoDocToSync.Pedido)
                            resp = ((SapOrder)sapDiapiObj).Add((OrdenMsg)docToSync);
                        if (tipoDocToSync==eTipoDocToSync.Cobro)
                            resp = ((SapPagoRecibido)sapDiapiObj).SafePago((PagosMsg)docToSync);
                        if (resp == "ok")
                        {
                            NofifySyncStatus(docToSync, tipoDocToSync.ToString()+ " registrado en SAP correctamente!!");
                            MoveToHistorico(docToSync);
                            docToSync.FechaIngresoSap = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                            onDocSyncOK?.Invoke(docToSync);
                        }
                        else //se notifica el error
                        {
                            NofifySyncStatus(docToSync, "Registrando err en bdd");
                            RegistrarErrEnCache(this.docToSyncActual, resp);
                            NotificarErrorPorCorreo(this.docToSyncActual, resp);
                            NofifySyncStatus(this.docToSyncActual, resp, eTipoMsg.Error);
                        }
                    }
                    sapDiapiObj.Disconnect();
                }
                catch (Exception e)
                {
                    sapDiapiObj.Disconnect();
                    RaiseError(e.Message + e.StackTrace);
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
            return ms;
        }
        private void RegistrarErrEnCache(DocsToSyncMsg docToSync, string err)
        {
            if (docToSync != null && docToSync.IdCache !=null) {
                try
                {
                    var sql = string.Format(@"
                        update JB_CACHE_DOCS_VET_TO_SYNC
                        set ERROR=?
                        where ID=?
                    ");
                    new BaseCore().Execute(sql, new Dictionary<string, object> {
                        {"@0", err },
                        {"@1", docToSync.IdCache },
                    });
                    NotificarErrorPorCorreo(docToSync, err);
                }
                catch (Exception e)
                {
                    NofifySyncStatus(docToSync,e.Message + e.StackTrace,eTipoMsg.Error);
                }
            }

            
        }
        private void NotificarErrorPorCorreo(DocsToSyncMsg doc, string err)
        {
            var titulo = "Error en sincronización de doc VET";
            var msg = string.Format(@"
                <h1>{6}</h1>
                <div>
                    <p><b>Id:<b> {0}</p>
                    <p><b>Tipo:<b> {1}</p>
                    <p><b>Fecha Sincronización Vendedor:<b> {2}</p>
                    <p><b>Cliente:<b> {3}</p>
                    <p><b>Vendedor:<b> {4}</p>
                    <p><b>Monto:<b> {5}</p>
                </div>",
                    doc.IdCache,
                    doc.TipoDocumento,
                    doc.FechaSincronizacionVendedor,
                    doc.Cliente,
                    doc.Vendedor,
                    doc.Total,
                    titulo);
            string error = null;
            this.EnviarPorCorreo(conf.Default.correoErrorSync, titulo, msg, ref error);
        }
        private DocsToSyncMsg docToSyncActual;
        internal void MoveToHistorico(DocsToSyncMsg me)
        {
            try
            {
                NofifySyncStatus(me, "Moviendo pedido a histórico...");
                var strMsg = TechTools.Serializador.SerializadorJson.Serializar(me);
                string strTotal = me.Total.ToString();
                strTotal = strTotal.Replace(",", ".");
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
            // para que no se dupliquen los mensajes
            if (docToSync.MensajesSincronizacion.Exists(ms => ms.Msg == msg))
                return;
            //concatenar con el status anterior
            docToSync.MensajesSincronizacion.Add(new MsgSincronizacion {

                Key= Guid.NewGuid().ToString(),
                FechaLog = DateTime.Now,
                Msg = msg,
                TipoMsg = tipoMsg,
            }); 
            onChangeSyncStatus?.Invoke(docToSync);
        }
        public List<DocsToSyncMsg> GetDocsVetToSyncByTipo(eTipoDocToSync tipoDocToSync)
        {
            var ms = new List<DocsToSyncMsg>();
            try
            {
                var sql = string.Format(@"
                    SELECT
                     ID,
                     CLIENTE,
                     VENDEDOR,
                     to_char(FECHA_SINCRONIZACION, 'YYYY-MM-DD HH24:MI:SS') ""FECHA_SINCRONIZACION"",
                     MSG FROM JB_CACHE_DOCS_VET_TO_SYNC
                    where 
                      PROCESANDO=0
                      and TIPO_DOC=?
                ");
                var bc = new BaseCore();
                var dt = bc.GetDataTableByQuery(sql, new Dictionary<string, object> {
                    {"@0",tipoDocToSync.ToString() }
                });
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        var id = dr["ID"].ToString();
                        var fechaSincronizacionVendedor = dr["FECHA_SINCRONIZACION"].ToString();
                        var cliente = dr["CLIENTE"].ToString();
                        var vendedor = dr["VENDEDOR"].ToString();
                        ActualizarEstado(id, "1");
                        var msgJson = dr["MSG"].ToString();
                        
                        var docToSync = GetObjToSyncByType(tipoDocToSync, id, fechaSincronizacionVendedor, cliente, vendedor, msgJson);
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

        private DocsToSyncMsg GetObjToSyncByType(eTipoDocToSync tipoDocToSync, string id, string fechaSincronizacionVendedor, string cliente, string vendedor, string msgJson)
        {
            DocsToSyncMsg ms = null;
            if (tipoDocToSync == eTipoDocToSync.Pedido)
            {
                ms = GetOrderdenMsgFromStrJson(msgJson);
            }
            if (tipoDocToSync == eTipoDocToSync.Cobro)
            {
                ms = GetCobroMsgFromStrJson(msgJson);
            }
            if (ms!=null)
            {
                
                ms.IdCache = id;
                ms.FechaSincronizacionVendedor = fechaSincronizacionVendedor;
                ms.Cliente = cliente;
                ms.Vendedor = vendedor;
                ms.MensajesSincronizacion = new List<MsgSincronizacion>();
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
                     MSG
                    from
                     JB_HISTORICO_DOCS_SINCRONIZADOS  
                    where
                     upper(VENDEDOR) like ?
                     and upper(CLIENTE) like ?
                     and FECHA_SINCRONIZACION >= TO_DATE(?,'yyyy-mm-dd')
					 AND FECHA_SINCRONIZACION <  ADD_DAYS(TO_DATE(?,'yyyy-mm-dd'),1)
                     and upper(TIPO_DOC) like ?
                    order by FECHA_SINCRONIZACION desc
                ");
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
                        
                        var idCache = dr["ID"].ToString();
                        var tipoDocumento= dr["TIPO_DOC"].ToString();
                        var cliente=dr["CLIENTE"].ToString();
                        var vendedor= dr["VENDEDOR"].ToString();
                        var msgJsonObj = dr["MSG"].ToString();
                        var fechaSincronizacionVendedor = dr["FECHA_SINCRONIZACION"].ToString();
                        var eTipoDocumento = eTipoDocToSync.NoDefinido;
                        if (tipoDocumento == eTipoDocToSync.Pedido.ToString())
                            eTipoDocumento = eTipoDocToSync.Pedido;
                        if (tipoDocumento == eTipoDocToSync.Cobro.ToString())
                            eTipoDocumento = eTipoDocToSync.Cobro;
                        var docToSync = GetObjToSyncByType(eTipoDocumento, idCache, fechaSincronizacionVendedor, cliente, vendedor, msgJsonObj);
                        if (docToSync != null) {
                            docToSync.FechaIngresoSap = dr["FECHA_INGRESO_SAP"].ToString();
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
    }
}
