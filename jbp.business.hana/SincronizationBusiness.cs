using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jbp.msg.sap;
using jbp.core.sapDiApi;
using TechTools.Core.Hana;
using System.Data;

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
                /*
                 Primero sincroniza los pedidos, y reutiliza la misma conexión a sap 
                 para sincronizar los cobros
                */
                var pedidos = GetOrderToSync();
                if (pedidos != null && pedidos.Count > 0)
                {
                    this.pedidoActual = pedidos[0];
                    var cantOrdenes = pedidos.Count;
                    if (cantOrdenes == 0)
                    {
                        NotifyMsg("No existen pedidos por sincronizar");
                        return;
                    }
                    NotificarPedidosToSync(pedidos);
                    SyncPedidos(pedidos);
                }
            }
            catch (Exception e) {
                RaiseError(e.Message + e.StackTrace);
            }
            
        }
        
        private static void ActualizarEstado(string id, string estado)
        {
            var sql = string.Format(@"
                update JB_CACHE_PEDIDOS
                set PROCESANDO={0}
                where ID={1}
            ", estado, id);
            new BaseCore().Execute(sql);
        }
        private OrdenMsg GetOrderdenMsgFromStrJson(string strJson, string idCache, string fechaSyncVendedor)
        {
            var order = TechTools.Serializador.SerializadorJson.Deserializar(typeof(OrdenMsg), strJson);
            var ms = (OrdenMsg)order;
            ms.IdCache = idCache;

            ms.FechaSincronizacionVendedor = fechaSyncVendedor;
            ms.Lines.ForEach(line => {
                if (string.IsNullOrEmpty(line.Articulo) && !string.IsNullOrEmpty(line.CodArticulo))
                    line.Articulo = ProductBusiness.GetNombreArticuloByCodigo(line.CodArticulo);
                if (line.price > 0)
                    line.price = Math.Round(line.price, 2);
            });
            return ms;
        }

        private void RegistrarErrEnCache(OrdenMsg pedido, string err)
        {
            if (pedido != null && pedido.IdCache !=null) {
                try
                {
                    var sql = string.Format(@"
                        update JB_CACHE_PEDIDOS
                        set ERROR='{0}'
                        where ID={1}
                    ", err,pedido.IdCache);
                    new BaseCore().Execute(sql);
                    NotificarErrorPorCorreo(GetPedidoStrResumen(pedido) + err);
                }
                catch (Exception e)
                {
                    NofifySyncStatus(pedido,e.Message + e.StackTrace,eTipoMsg.Error);
                }
            }

            
        }

        private string GetPedidoStrResumen(OrdenMsg me)
        {
            return string.Format("Cliente: {0}, Vendedor: {1}, Monto: {2}, FechaSyncVendedor: {3}",
                me.Cliente, me.Vendedor, me.Total, me.FechaSincronizacionVendedor);
        }

        private void NotificarErrorPorCorreo(string err)
        {
        }

        private OrdenMsg pedidoActual;
        private void SyncPedidos(List<OrdenMsg> pedidos)
        {
            var sapPedido = new jbp.core.sapDiApi.SapOrder();
            this.pedidoActual = pedidos[0];
            sapPedido.onNotififacationMessage += (msg) => {
                NofifySyncStatus(this.pedidoActual,msg);
            };
            if (sapPedido.Connect())
            {
                try
                {
                    foreach (var pedido in pedidos)
                    {
                        this.pedidoActual = pedido;
                        var resp = sapPedido.Add(pedido);
                        if (resp == "ok")
                        {
                            NofifySyncStatus(pedido, "Pedido registrado en SAP correctamente!!");
                            MoveToHistorico(pedido);
                            onDocSyncOK?.Invoke(pedido);
                        }
                        else
                        {
                            NofifySyncStatus(pedido, "Registrando err en bdd");
                            RegistrarErrEnCache(this.pedidoActual, resp);
                            NotificarErrorPorCorreo(resp);
                            NofifySyncStatus(this.pedidoActual, resp, eTipoMsg.Error);
                        }
                    }
                    sapPedido.Disconnect();
                }
                catch (Exception e)
                {
                    sapPedido.Disconnect();
                    RaiseError(e.Message + e.StackTrace);
                }
            }
        }
        internal void MoveToHistorico(OrdenMsg me)
        {
            try
            {
                NofifySyncStatus(me, "Moviendo pedido a histórico...");
                var strMsg = TechTools.Serializador.SerializadorJson.Serializar(me);
                string strTotal = me.Total.ToString();
                strTotal = strTotal.Replace(",", ".");
                var sql = string.Format(@"
                    insert into JB_HISTORICO_PEDIDOS(ID, FECHA_SINCRONIZACION, VENDEDOR, CLIENTE, MONTO, MSG)
                    values ({0},'{1}', '{2}', '{3}',{4}, '{5}')
                ", me.IdCache, me.FechaSincronizacionVendedor, me.Vendedor, me.Cliente, strTotal, strMsg);
                new BaseCore().Execute(sql);
                // se borra el pedido sincronizado de cache
                NofifySyncStatus(me, "Eliminando pedido del caché...");
                sql = string.Format(@"
                    delete from JB_CACHE_PEDIDOS where ID={0}
                ", me.IdCache);
                new BaseCore().Execute(sql);
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

        private void NotificarPedidosToSync(List<OrdenMsg> pedidos)
        {
            var docsToSync = new List<DocsToSyncMsg>();
            pedidos.ForEach(pedido =>
            {
                docsToSync.Add(pedido);
            });
            onDocsToSync?.Invoke(docsToSync);
        }

        public List<OrdenMsg> GetOrderToSync()
        {
            var ms = new List<OrdenMsg>();
            try
            {
                var sql = string.Format(@"
                    SELECT
                     ID,
                     to_char(FECHA_SINCRONIZACION, 'YYYY-MM-DD HH24:MI:SS') ""FECHA_SINCRONIZACION"",
                     MSG FROM JB_CACHE_PEDIDOS
                    where PROCESANDO=0
                ");
                var bc = new BaseCore();
                var dt = bc.GetDataTableByQuery(sql);
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        var id = dr["ID"].ToString();
                        ActualizarEstado(id, "1");
                        var msgJson = dr["MSG"].ToString();
                        var pedido = GetOrderdenMsgFromStrJson(msgJson, id, dr["FECHA_SINCRONIZACION"].ToString());
                        pedido.MensajesSincronizacion = new List<MsgSincronizacion>();
                        ms.Add(pedido);
                    }
                }
            }
            catch (Exception e)
            {
                RaiseError(e.Message + e.StackTrace);
            }
            return ms;
        }
        public List<DocsToSyncMsg> ConsultarHistoricoPedidos(FiltroHistoricoMsg filtro)
        {
            var ms = new List<DocsToSyncMsg>();
            try
            {
                
                var desde = filtro.Desde.ToString("yyyy-MM-dd");
                var hasta = filtro.Hasta.ToString("yyyy-MM-dd");
                var sql = string.Format(@"
                    select
                     ID,
                     to_char(FECHA_SINCRONIZACION,'yyyy-mm-dd hh24:mi:ss') FECHA_SINCRONIZACION,
                     to_char(FECHA_INGRESO_SAP,'yyyy-mm-dd hh24:mi:ss') FECHA_INGRESO_SAP,
                     VENDEDOR,
                     CLIENTE,
                     MONTO,
                     MSG
                    from
                     JB_HISTORICO_PEDIDOS  
                    where
                     upper(VENDEDOR) like '%{0}%'
                     and upper(CLIENTE) like '%{1}%'
                     and FECHA_SINCRONIZACION >= TO_DATE('{2}','yyyy-mm-dd')
						AND   FECHA_SINCRONIZACION <  ADD_DAYS(TO_DATE('{3}','yyyy-mm-dd'),1);
                ", filtro.Vendedor.ToUpper(), filtro.Cliente.ToUpper(), desde, hasta);
                var dt = new BaseCore().GetDataTableByQuery(sql);
                if (dt.Rows != null && dt.Rows.Count > 0) {
                    foreach (DataRow dr in dt.Rows)
                    {
                        var jsonPedido = dr["MSG"].ToString();
                        var idCache = dr["ID"].ToString();
                        var fechaSincronizacionVendedor = dr["FECHA_SINCRONIZACION"].ToString();
                        var pedido = GetOrderdenMsgFromStrJson(jsonPedido, idCache, fechaSincronizacionVendedor);
                        pedido.FechaIngresoSap= dr["FECHA_INGRESO_SAP"].ToString();
                        ms.Add(pedido);
                    }
                }
            }
            catch (Exception e) { 
                RaiseError(e.Message+e.StackTrace);
            }
            return ms;
        }
    }
}
