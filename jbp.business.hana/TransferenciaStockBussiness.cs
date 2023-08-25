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
using jbp.msg.sap;

namespace jbp.business.hana
{
    public class TransferenciaStockBussiness
    {
        public static readonly object control = new object();
        public static SapTransferenciaStock sapTransferenciaStock = new SapTransferenciaStock();

        #region Desde solicitud de transferencia
        public static DocSapInsertadoMsg SaveFromST(TsFromPickingME me)
        {
            Monitor.Enter(control);
            try
            {
                var ms = ProcessTSFromST(me);
                if (string.IsNullOrEmpty(ms.Error))
                {
                    ms.DocNum = GetDocNumBYId(ms.Id);
                    UpdateResponsableTS(me.Responsable, ms.Id);
                }
                return ms;
            }
            finally
            {
                Monitor.Exit(control);
            }
        }
        public static List<LotesCuarMS> GetLotesCuarentena() { 
            var ms = new List<LotesCuarMS>();
            var sql = @"
                select 
                 t0.""CodArticulo"",
                 t1.""Lote"",
                 t1.""Estado"",
                 t0.""CodBodega"",
                 round(t0.""Cantidad"",4) ""Cantidad""
                from
                 ""JbpVw_CantidadesPorLote"" t0 inner join
                 ""JbpVw_Lotes"" t1 on t1.""Id"" = t0.""IdLote"" and t0.""CodArticulo"" = t1.""CodArticulo""
                where
                  t0.""CodBodega"" like 'CUAR%'
                  and t0.""Cantidad"" != 0
            ";
            var bc = new BaseCore();
            var dt = bc.GetDataTableByQuery(sql);
            foreach (DataRow dr in dt.Rows) {
                ms.Add(new LotesCuarMS {
                    CodArticulo = dr["CodArticulo"].ToString(),
                    Lote = dr["Lote"].ToString(),
                    Estado = dr["Estado"].ToString(),
                    CodBodega = dr["CodBodega"].ToString(),
                    Cantidad = dr["Cantidad"].ToString()
                });
            }
            return ms;
        }
        //sistema balanzas espinoza paez
        public static DocSapInsertadoMsg TransferFromBalanzas(TsBalanzasMsg me)
        {
            var ms = new DocSapInsertadoMsg();
            try
            {
                if (me.DocNumOF > 0)
                {
                    ms = TS_ConLotes(me);
                    if (string.IsNullOrEmpty(ms.Error))
                    {
                        UpdateResponsableTS("Sistema Balanzas Espinoza Paez", ms.Id);
                        setOfComoFraccionada(me.DocNumOF);
                    }
                }
                else
                    throw new Exception("No se ha pasado como parámetro en número de orden de fabricación 'DocNumOF'");
                
            }
            catch (Exception e)
            {
                ms.Error = e.Message;
                
            }
            return ms;
        }
        public static DocSapInsertadoMsg TS_ConLotes(TsBalanzasMsg me)
        {

            Monitor.Enter(control);
            var ms = new DocSapInsertadoMsg();
            try
            {
                if (me != null)
                {
                    if (sapTransferenciaStock == null)
                        sapTransferenciaStock = new SapTransferenciaStock();
                    if (!sapTransferenciaStock.IsConected())
                        sapTransferenciaStock.Connect();//se conecta a sap
                    ms = sapTransferenciaStock.TransferirSinUbicaciones(me);
                }
                if (string.IsNullOrEmpty(ms.Error))
                {
                    ms.DocNum = GetDocNumBYId(ms.Id);
                }
            }
            catch (Exception e)
            {
                ms.Error = e.Message;

            }
            Monitor.Exit(control);
            return ms;
        }

        private static DocSapInsertadoMsg ProcessTSFromST(TsFromPickingME me)
        {
            var ms= new DocSapInsertadoMsg();
            try
            {
                if (sapTransferenciaStock == null)
                    sapTransferenciaStock = new SapTransferenciaStock();
                if (!sapTransferenciaStock.IsConected())
                    sapTransferenciaStock.Connect();//se conecta a sap
                me.Componentes.ForEach(c => { 
                    c.IdUbicacion = BodegaBusiness.GetIdUbicacionByName(c.ubicacionSeleccionada);
                });
                ms= sapTransferenciaStock.AddFromSt(me);
                
            }
            catch (Exception e)
            {
                ms.Error=e.Message;
            }
            return ms;
        }

        private static void setOfComoFraccionada(int docNumOf)
        {
            if (docNumOf > 0) {
                var sql = string.Format(@"update OWOR set ""U_JbFraccionadoPesaje""='SI' where ""DocNum""={0}", docNumOf);
                new BaseCore().Execute(sql);
            }
        }

        private static int GetIdSTFromDocNumOF(int docNum)
        {
            var sql = string.Format(@"
                select ""Id"" from ""vw_STConDocNumOF""
                where ""DocNumOF"" = '{0}'
            ");
            var bc = new BaseCore();
            var dt = bc.GetDataTableByQuery(sql);
            if (dt.Rows.Count > 1)
                throw new Exception("La Orden de fabriación " + docNum + " tiene mas de una Solicitud de transferencia activa!!");
            if (dt.Rows.Count == 1)
                return bc.GetInt(dt.Rows[0]["Id"]);
            else
                return 0;
        }
        #endregion

        // app bodega
        public static DocSapInsertadoMsg TransferToUbicaciones(TsBodegaMsg me)
        {
            Monitor.Enter(control);
            try{
                if (!string.IsNullOrEmpty(me.UbicacionDesde)) { //si se envia la ubicación destino explicitamente
                    me.IdUbicacionDesde = BodegaBusiness.GetIdUbicacionByName(me.UbicacionDesde);
                }
                me.CantidadTotal = 0;
                foreach (var uch in me.UbicacionesCantidadHasta) {
                    if (!string.IsNullOrEmpty(me.UbicacionDesde) && uch.Ubicacion == me.UbicacionDesde) {
                        return new DocSapInsertadoMsg
                        {
                            Error = "Error: La ubicación destino no puede ser igual a la de origen (" + uch.Ubicacion + ")"
                        };
                    }
                    uch.IdUbicacion = BodegaBusiness.GetIdUbicacionByName(uch.Ubicacion);
                    me.CantidadTotal += uch.Cantidad;
                }
                var estadoLote = getEstadoLote(me); 
                /*
                 solo se permiten hacer transferencias de stock en lotes liberados
                 Por esto para que desde bodega puedan mover los artículos temporalmente 
                 se cambia el estado del lote a liberado, luego se vuelve a poner el estado
                 anterior del lote
                */
                if(estadoLote!=Convert.ToInt32(eEstadoLote.Liberado).ToString())
                    CambiarEstadoLote(eEstadoLote.Liberado, me);
                var ms = ProcessTS(me);
                if (estadoLote != Convert.ToInt32(eEstadoLote.Liberado).ToString())
                    CambiarEstadoLote(estadoLote, me);//vuelte al estado anterior
                if (string.IsNullOrEmpty(ms.Error)) {
                    ms.DocNum = GetDocNumBYId(ms.Id);
                    UpdateResponsableTS(me.Responsable, ms.Id);
                }
                return ms;
            }
            finally{
                Monitor.Exit(control);
            }
        }
        private static string getEstadoLote(TsBodegaMsg me)
        {
            var sql = string.Format(@"
             select ""Status""
             from OBTN
             where 
              ""DistNumber""='{0}'
              and ""ItemCode""='{1}'
            ", me.Lote, me.CodArticulo);
            var ms=new BaseCore().GetScalarByQuery(sql);
            return ms;
        }
        private static void CambiarEstadoLote(eEstadoLote estado, TsBodegaMsg me) {
            CambiarEstadoLote(Convert.ToInt32(estado).ToString(), me);
        }
        private static void CambiarEstadoLote(string estado, TsBodegaMsg me)
        {
            var sql = string.Format(@"
             update OBTN
              set ""Status""={0}
             where 
              ""DistNumber""='{1}'
              and ""ItemCode""='{2}'
            ", estado, me.Lote, me.CodArticulo);
            new BaseCore().Execute(sql);
        }
        private static void UpdateResponsableTS(string responsable, string id)
        {
            var sql = string.Format(@"
                update OWTR
                set ""Comments""='** Responsable Ingreso: {0} **   ' ||  ""Comments""
                where ""DocEntry"" = {1}
            ", responsable, id);
            new BaseCore().Execute(sql);
        }

        private static int GetDocNumBYId(string id)
        {
            var sql = string.Format(@"
            select
             ""DocNum""
            from
             ""JbpVw_TransferenciaStock""
            where
             ""Id"" = {0}
            ", id);
            return new BaseCore().GetIntScalarByQuery(sql);
        }

        /*private static void SetLotesFEFO(TsBodegaLineaMsg line, string CodBodegaDesde)
        {
            
            var cantlotes = GetLotesByCodArt_CodBodega(line.CodArticulo, CodBodegaDesde);
            double asignado = 0;
            double porAsignar = line.Cantidad;
            cantlotes.ForEach(cl =>
            {
                if (porAsignar > 0) {
                    if (cl.Disponible <= porAsignar){
                        asignado = cl.Disponible;
                        porAsignar -= cl.Disponible;
                    }
                    else{
                        asignado = porAsignar;
                        porAsignar = 0;
                    }
                    line.Lotes.Add(new AsignacionLoteMsg{
                        Lote = cl.Lote,
                        Cantidad = asignado
                    });
                }
                    
            });
            //si no hay suficiente cantidad en lotes para transferir se lanza una excepcion
            if (porAsignar > 0) {
                var err = string.Format("No existe cantidad en lotes suficiente para transferir {0} unidades del artículo {1}", line.Cantidad, line.CodArticulo);
                if (cantlotes.Count > 0) {
                    err += "Información de lotes encontrados: ";
                    cantlotes.ForEach(cl => {
                        err += string.Format("Lote: {0}, Cantidad: {1}, Bodega: {2} ",cl.Lote, cl.Disponible, cl.CodBodega);
                    });
                }
                throw new Exception(err);
            }
            
        }*/

        private static List<CantidadLoteMsg> GetLotesByCodArt_CodBodega(string codArticulo, string codBodegaDesde)
        {
            var ms = new List<CantidadLoteMsg>();
            var sql = string.Format(@"
                select
                 ""Lote"",
                 ""Disponible"",
                 ""CodAlmacen""
                from ""JbpVw_CantDisponibleArticuloPorLotes""
                where
                 ""CodArticulo"" = '{0}'
                 and ""CodAlmacen"" = '{1}'
                 and  ""Disponible"" > 0
                 and ""FechaVencimiento"">=current_Date --lotes que no estén vencidos
                order by
                 ""FechaVencimiento"" -- Desde lo que esta mas próximo a vencerse
            ", codArticulo, codBodegaDesde);
            var bc = new BaseCore();
            var dt=bc.GetDataTableByQuery(sql);
            foreach (DataRow dr in dt.Rows) {
                ms.Add(new CantidadLoteMsg
                {
                    Lote = dr["Lote"].ToString(),
                    Disponible = bc.GetDouble(dr["Disponible"]),
                    CodBodega =dr["CodAlmacen"].ToString()
                });
            }
            return ms;
        }

        private static DocSapInsertadoMsg ProcessTS(TsBodegaMsg me)
        {
            try
            {
                if (me != null)
                {
                    if (sapTransferenciaStock == null)
                        sapTransferenciaStock = new SapTransferenciaStock();
                    if (!sapTransferenciaStock.IsConected())
                        sapTransferenciaStock.Connect();//se conecta a sap
                    return sapTransferenciaStock.TranferirEntreUbicaciones(me);
                }
                return null;
            }
            catch (Exception e)
            {
                return new DocSapInsertadoMsg { 
                    Error = e.Message
                };
            }
        }
    }
}
