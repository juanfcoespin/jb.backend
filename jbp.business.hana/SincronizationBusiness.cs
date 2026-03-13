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
             Primero sincroniza los pedidos, y reutiliza la misma conección a sap 
             para sincronizar los cobros
             */
                var ordenBussines = new jbp.business.hana.OrderBusiness();
                ordenBussines.onError += (string err) =>
                {
                    if (this.pedidoActual != null)
                    {
                        NofifySyncStatus(this.pedidoActual, err, eTipoMsg.Error);
                        RegistrarErrEnCache(this.pedidoActual, err);
                    }
                    else
                        RaiseError(err);
                    NotificarErrorPorCorreo(err);
                };
                var pedidos = ordenBussines.GetOrderToSync();
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
                this.pedidoActual.Status += string.Format("{0}: {1}", DateTime.Now.ToString(), msg);
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
                            var orderBusiness = new OrderBusiness();
                            orderBusiness.onError += (msg) => { NofifySyncStatus(this.pedidoActual, msg, eTipoMsg.Error); };
                            orderBusiness.MoveToHistorico(pedido);
                            onDocSyncOK?.Invoke(pedido);
                        }
                        else
                        {
                            NofifySyncStatus(this.pedidoActual, resp, eTipoMsg.Error);
                            RegistrarErrEnCache(this.pedidoActual, resp);
                        }
                    }
                }
                catch {
                    sapPedido.Disconnect();
                }
            }
        }

        private void NofifySyncStatus(DocsToSyncMsg docToSync, string msg, eTipoMsg tipoMsg=eTipoMsg.Info)
        {
            //concatenar con el status anterior
            docToSync.MensajesSincronizacion.Add(new MsgSincronizacion {
                FechaLog= DateTime.Now,
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

        public List<OrdenMsg> ConsultarHistoricoPedidos(FiltroHistoricoMsg filtro)
        {
            var ms = new List<OrdenMsg>();
            try
            {
                
                var desde = filtro.Desde.ToString("yyyy-MM-dd");
                var hasta = filtro.Hasta.ToString("yyyy-MM-dd");
                var sql = string.Format(@"
                    select
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
                        var obj = TechTools.Serializador.SerializadorJson.Deserializar(typeof(OrdenMsg), jsonPedido);
                        OrdenMsg pedido =(OrdenMsg)obj;
                        pedido.Lines.ForEach(line => {
                            line.Articulo = ProductBusiness.GetNombreArticuloByCodigo(line.CodArticulo);
                        });
                        pedido.FechaSincronizacionVendedor = dr["FECHA_SINCRONIZACION"].ToString();
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
