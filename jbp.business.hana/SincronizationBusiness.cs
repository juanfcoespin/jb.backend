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
        public void SincronizarPedidoYCobros(List<OrdenMsg> pedidos) {
            /*
             Primero sincroniza los pedidos, y reutiliza la misma conección a sap 
             para sincronizar los cobros
             */
            var sapPedido = new jbp.core.sapDiApi.SapOrder();
            sapPedido.onNotififacationMessage+=(msg) => { NotifyMsg(msg); };
            if (sapPedido.Connect()) {
                foreach (var pedido in pedidos)
                {
                    var resp=sapPedido.Add(pedido);
                    if (resp == "ok") {
                        var orderBusiness = new OrderBusiness();
                        orderBusiness.onError += (msg) => { RaiseError(msg); };
                        orderBusiness.MoveToHistorico(pedido);
                    }
                }
                sapPedido.Disconnect();
            }
        }
    }
}
