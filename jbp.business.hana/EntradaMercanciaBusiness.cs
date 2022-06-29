using jbp.msg.sap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jbp.core.sapDiApi;
using System.Threading;
using TechTools.Core.Hana;
using System.Data;

namespace jbp.business.hana
{
    public class EntradaMercanciaBussiness
    {
        public static readonly object control = new object();
        public static SapEntradaMercancia sapEntradaMercancia = new SapEntradaMercancia();

        public static EntradaMercanciaMsg Ingresar(EntradaMercanciaMsg me, bool porCompra)
        {
            Monitor.Enter(control);
            try
            {
                var ms = ProcessEM(me, porCompra);
                return ms;
            }
            finally
            {
                Monitor.Exit(control);
            }
        }

        private static EntradaMercanciaMsg ProcessEM(EntradaMercanciaMsg me, bool porCompra)
        {
            try
            {
                if (me != null)
                {
                    
                    if (sapEntradaMercancia == null)
                        sapEntradaMercancia = new SapEntradaMercancia();
                    if (!sapEntradaMercancia.IsConected())
                        sapEntradaMercancia.Connect();//se conecta a sap
                    SetNewLotesAndBodega(me); //se asigna los lotes nuevos desde la bdd
                    if (porCompra)
                        SetAdicionalesPedidoCompra(me); //pone el id y el codProveedor del pedido de compra
                    
                    var ms=(porCompra)? sapEntradaMercancia.AddPorCompra(me): sapEntradaMercancia.Add(me);
                    if (string.IsNullOrEmpty(ms.Error)) { //si no hay error
                        ms.DocNumEntradaMercancia = GetDocNumById(ms.IdEM);
                        UpdateResponsableEM(me.responsable, ms.IdEM);
                    }
                    return ms;
                }
                return null;
            }
            catch (Exception e)
            {
                return new EntradaMercanciaMsg { Error = e.Message };
            }
        }

        private static void UpdateResponsableEM(string responsable, string idEM)
        {
            var sql = string.Format(@"
                update OPDN
                set ""Comments""='** Responsable Ingreso: {0} **   ' ||  ""Comments""
                where ""DocEntry"" = {1}
            ",responsable,idEM );
            new BaseCore().Execute(sql);
        }

        public static int GetDocNumById(string idEntradaMercancia) {
            var sql = string.Format(@"
            select
             ""DocNum""
            from
             ""JbpVw_EntradaMercancia""
            where
             ""Id"" = {0}
            ", idEntradaMercancia);
            return new BaseCore().GetIntScalarByQuery(sql);
        }
        private static void SetAdicionalesPedidoCompra(EntradaMercanciaMsg me)
        {
            var sql = string.Format(@"
            select
             ""Id"",
             ""CodProveedor""
            from
             ""JbpVw_Pedidos""
            where
             ""DocNum"" = {0}
            ", me.NumPedido);
            var bc = new BaseCore();
            var dt=bc.GetDataTableByQuery(sql);
            if (dt != null) {
                me.IdOrdenCompra = bc.GetInt(dt.Rows[0]["Id"]);
                me.CodProveedor= dt.Rows[0]["CodProveedor"].ToString();
            }
        }

        private static void SetNewLotesAndBodega(EntradaMercanciaMsg me)
        {
            //Ej.: JB-220530142909
            // Se genera en la base de datos para garantizar que no se duplique el lote
            // (con la fecha y hora del servidor de base de datos)
            me.Lineas.ForEach(line => {
                if (string.IsNullOrEmpty(line.CodBodega))
                    line.CodBodega = "CUAR1"; //por defecto se va a cuarentena a no ser que el usuario mande a otra bodega
                line.AsignacionesLote.ForEach(al => {
                    if (string.IsNullOrEmpty(al.Lote))
                    {
                        var sql = "call SBO_SP_LOTES_OP('EP','');"; // se pasa como parámetro EP, para indicar a la base que es EM por compra
                        al.Lote = new BaseCore().GetScalarByQuery(sql);
                        System.Threading.Thread.Sleep(1000); //para que se genere un nuevo lote
                    }
                });
            });
            
        }
    }
}
