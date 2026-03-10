using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jbp.msg.sap;
using jbp.core.sapDiApi;

namespace jbp.business.hana
{
    public class SincronizationBusiness:BaseBusiness
    {
        public delegate void dDocsToSync(List<DocsToSyncMsg> docs);
        public event dDocsToSync onDocsToSync;

        public delegate void dNotyfySyncStatus(DocsToSyncMsg doc);
        public event dNotyfySyncStatus onChangeSyncStatus;

        public void SincronizarPedidoYCobros()
        {
            /*
             Primero sincroniza los pedidos, y reutiliza la misma conección a sap 
             para sincronizar los cobros
             */
            var ordenBussines = new jbp.business.hana.OrderBusiness();
            ordenBussines.onError += (string err) =>
            {
                if (this.pedidoActual != null)
                    NofifySyncStatus(this.pedidoActual, err, eTipoMsg.Error);
                else
                    RaiseError(err);
                NotificarErrorPorCorreo(err);
            };
            var pedidos = ordenBussines.GetOrderToSync();
            if (pedidos != null && pedidos.Count > 0) {
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

        private void NotificarErrorPorCorreo(string err)
        {
            throw new NotImplementedException();
        }

        private OrdenMsg pedidoActual;
        private void SyncPedidos(List<OrdenMsg> pedidos)
        {
            var sapPedido = new jbp.core.sapDiApi.SapOrder();
            this.pedidoActual = pedidos[0];
            sapPedido.onNotififacationMessage += (msg) => { 
                NofifySyncStatus(this.pedidoActual, msg);
            };
            if (sapPedido.Connect())
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
                    }
                }
                sapPedido.Disconnect();
            }
        }

        private void NofifySyncStatus(OrdenMsg pedido, string msg, eTipoMsg tipoMsg=eTipoMsg.Info)
        {
            var docStatus = GetDocSatusFromPedido(pedido);
            docStatus.Status += string.Format("{0}: {1}", DateTime.Now.ToString(), msg);
            docStatus.TipoMsg=tipoMsg;
            onChangeSyncStatus?.Invoke(docStatus);
        }

        private static DocsToSyncMsg GetDocSatusFromPedido(OrdenMsg pedido)
        {
            return new DocsToSyncMsg
            {
                IdCache = pedido.IdCache,
                FechaSincronizacionVendedor = pedido.FechaSincronizacionVendedor,
                TipoDocumento = "Pedido de Venta",
                Cliente = pedido.Cliente,
                Vendedor = pedido.Vendedor,
                Monto = pedido.Total,
            };
        }

        private void NotificarPedidosToSync(List<OrdenMsg> pedidos)
        {
            var docsToSync = new List<DocsToSyncMsg>();
            pedidos.ForEach(pedido =>
            {
                docsToSync.Add(GetDocSatusFromPedido(pedido));
            });
            onDocsToSync?.Invoke(docsToSync);
        }

        public void ConsultarHistorico(FiltroHistoricoMsg filtro)
        {
            
        }
    }
}
