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
        //sistema balanzas espinoza paez
        public static DocSapInsertadoMsg TransferFromBalanzas(TsBalanzasMsg me)
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
                    ms= sapTransferenciaStock.AddFromBalazas(me);
                }
                if (string.IsNullOrEmpty(ms.Error))
                {
                    ms.DocNum = GetDocNumBYId(ms.Id);
                    UpdateResponsableTS("Sistema Balanzas Espinoza Paez", ms.Id);
                }
            }
            catch (Exception e)
            {
                ms.Error = e.Message;
                
            }
            Monitor.Exit(control);
            return ms;
        }

        public static DocSapInsertadoMsg TransferFromPicking(TsFromPickingME me)
        {
            throw new NotImplementedException();
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
                var ms = ProcessTS(me);
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
                    return sapTransferenciaStock.TranferUbicaciones(me);
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
