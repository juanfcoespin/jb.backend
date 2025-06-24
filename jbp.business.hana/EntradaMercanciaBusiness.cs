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
                    //desde la app no se controla el envío de lineas sin asignacion lote
                    me.Lineas = me.Lineas.FindAll(l => l.AsignacionesLote.Count > 0);
                    if (me.Lineas == null)
                        throw new Exception("SRV: Ninguna línea de la entrada de mergancía tiene lotes asignados");
                    EntradaMercanciaMsg.SetCodBodegaEnLineas(me.Lineas); //se asigna el código de bodega a las líneas de la entrada de mercancia
                    SetNewLotes(me); //se asigna los lotes nuevos desde la bdd
                    me.Lineas.ForEach(line => {
                        line.AsignacionesLote.ForEach(al => {
                            if (!string.IsNullOrEmpty(al.Ubicacion)) { 
                                al.IdUbicacion= BodegaBusiness.GetIdUbicacionByName(al.Ubicacion);
                            }
                        });
                    });
                    SetGastosAdicionales(ref me);
                    var ms=(porCompra)? sapEntradaMercancia.AddPorCompra(me): sapEntradaMercancia.Add(me);
                    if (string.IsNullOrEmpty(ms.Error)) { //si no hay error
                        ms.DocNumEntradaMercancia = GetDocNumById(ms.IdEM);
                        SetComentarioAndUpdateResponsableEM(me, ms.IdEM);
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

        

        private static void SetGastosAdicionales(ref EntradaMercanciaMsg me)
        {
            if (me.tipo == "Factura de Reserva") {
                var sql = string.Format(@"
                   select 
                    ""ObjType"",
                    ""DocEntry"",
                    ""LineNum""
                     from PCH3 where ""DocEntry"" = {0}
                ", me.IdDocOrigen);
                var dt = new BaseCore().GetDataTableByQuery(sql);
                foreach (DataRow dr in dt.Rows) {
                    me.GastosAdicionales.Add(new GastosAdicionalesMsg { 
                        ObjType = dr["ObjType"].ToString(),
                        DocEntry = dr["DocEntry"].ToString(),
                        LineNum = dr["LineNum"].ToString()
                    });
                }
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

        private static void SetComentarioAndUpdateResponsableEM(EntradaMercanciaMsg me, string idEM)
        {
            var comentarioBdd = GetComentarioEM(idEM);
            me.Comentario = String.Format("(Ingresado por: {0}) {1} {2}",
                me.responsable, comentarioBdd, me.Comentario    
            );
            // se trunca el comentario a la longitud máxima en la bdd
            if( me.Comentario.Length > 253){
                me.Comentario = me.Comentario.Substring(0, 253);
            }
            var sql = string.Format(@"
                update OPDN
                set ""Comments""='{0}'
                where ""DocEntry"" = {1}
            ", me.Comentario, idEM );
            new BaseCore().Execute(sql);
        }

        private static string GetComentarioEM(string idEM)
        {
            var sql = string.Format(@"
                select ""Comments"" from OPDN where ""DocEntry"" = {0}
            ", idEM);
            return new BaseCore().GetScalarByQuery(sql);
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
