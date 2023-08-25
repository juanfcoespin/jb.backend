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
                    SetNewLotes(me); //se asigna los lotes nuevos desde la bdd
                    if (porCompra)
                        SetAdicionalesPedidoCompra(me); //pone el id y el codProveedor del pedido de compra
                    me.Lineas.ForEach(line => {
                        line.AsignacionesLote.ForEach(al => {
                            if (!string.IsNullOrEmpty(al.Ubicacion)) { 
                                al.IdUbicacion= BodegaBusiness.GetIdUbicacionByName(al.Ubicacion);
                            }
                        });
                    });
                    var ms=(porCompra)? sapEntradaMercancia.AddPorCompra(me): sapEntradaMercancia.Add(me);
                    if (string.IsNullOrEmpty(ms.Error)) { //si no hay error
                        ms.DocNumEntradaMercancia = GetDocNumById(ms.IdEM);
                        UpdateResponsableEM(me, ms.IdEM);
                        ms.Lineas.ForEach(l => {
                            l.AsignacionesLote.ForEach(al => {
                                ActualizarEstadoLoteCuarentena(al.Lote);
                            });
                            //registro la unidad de medida del articulo
                            me.Lineas.ForEach(lme => {
                                if (lme.CodArticulo == l.CodArticulo) {
                                    l.UnidadMedida = lme.UnidadMedida;
                                }

                            });
                        });
                        //InsertResumenEM(ms);
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

        private static void ActualizarEstadoLoteCuarentena(string loteJB)
        {
            /*
             En el objeto lote de la DIapi de sap no se puede gestionar el estado del lote
             por esto se lo hacer mediante una actualizacion a la base de datos

                Status 1 -> Acceso Denegado
             */
            if (!LoteEnCuarentena(loteJB))
            {
                var sql = string.Format(@"update OBTN set ""Status""=1 where ""DistNumber""='{0}'", loteJB);
                new BaseCore().Execute(sql);
            }
            
        }

        private static bool LoteEnCuarentena(string loteJB)
        {
            try
            {
                var sql = string.Format(@"
                 select 
                    ""Estado""
                 from
                    ""JbpVw_Lotes""
                 where
                    ""Lote""='{0}'
                ");
                var estado = new BaseCore().GetScalarByQuery(sql);
                return estado == "Acceso Denegado";
            }
            catch
            {
                return false;
            }
        }

        //private static void InsertResumenEM(EntradaMercanciaMsg ms)
        //{

        //}

        private static void UpdateResponsableEM(EntradaMercanciaMsg me, string idEM)
        {
            var sql = string.Format(@"
                update OPDN
                set ""Comments""='** {0} Responsable Ingreso: {1} **   ' ||  ""Comments""
                where ""DocEntry"" = {2}
            ", me.Comentario, me.responsable,idEM );
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

        private static void SetNewLotes(EntradaMercanciaMsg me)
        {
            //Ej.: JB-220530142909
            // Se genera en la base de datos para garantizar que no se duplique el lote
            // (con la fecha y hora del servidor de base de datos)
            me.Lineas.ForEach(line => {
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
