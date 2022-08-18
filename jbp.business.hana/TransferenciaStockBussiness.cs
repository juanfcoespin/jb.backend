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
        public static string SaveFromST(SalidaBodegaMsg me)
        {
            Monitor.Enter(control);
            try
            {
                var ms = ProcessTSFromST(me);
                return ms;
            }
            finally
            {
                Monitor.Exit(control);
            }
        }
        private static string ProcessTSFromST(SalidaBodegaMsg me)
        {
            try
            {
                if (me != null && me.DocNum > 0)
                {
                    me.IdOF = OrdenFabricacionBusiness.GetIdByDocNum(me.DocNum);
                    if (!OrdenFabricacionBusiness.EstaLiberada(me.IdOF))
                        return String.Format("La orden de fabricacion: {0} no está en estado liberada!!", me.DocNum);
                    if (me.DocBaseType == EDocBase.SolicitudTransferencia)
                    {
                        me.IdDocBase = GetIdSTFromDocNumOF(me.DocNum);
                        if (sapTransferenciaStock == null)
                            sapTransferenciaStock = new SapTransferenciaStock();
                        if (!sapTransferenciaStock.IsConected())
                            sapTransferenciaStock.Connect();//se conecta a sap
                        return sapTransferenciaStock.AddFromSt(me);
                    }

                }
                return null;
            }
            catch (Exception e)
            {
                return e.Message;
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
        public static DocSapInsertadoMsg Transfer(TsBodegaMsg me)
        {
            Monitor.Enter(control);
            try{
                if (!string.IsNullOrEmpty(me.UbicacionHasta)) { //si se envia la ubicación destino explicitamente
                    me.IdUbicacionHasta = BodegaBusiness.GetIdUbicacionByName(me.UbicacionHasta);
                }
                foreach (var line in me.Lineas) {
                    if (line.Lotes.Count == 0)// si no tiene lotes se asiga lotes segun FEFO
                        SetLotesFEFO(line, me.CodBodegaDesde);
                    if (line.Lotes.Count > 0)
                        line.Cantidad = 0;
                    foreach (var lote in line.Lotes) {
                        if(lote.UbicacionesCantidadDesde.Count>0)
                            lote.Cantidad = 0;
                        foreach (var uc in lote.UbicacionesCantidadDesde){//si se envían ubicacones explicitamente
                            if (!string.IsNullOrEmpty(me.UbicacionHasta) && uc.Ubicacion == me.UbicacionHasta) {
                                return new DocSapInsertadoMsg
                                {
                                   Error = "Error: La ubicación destino no puede ser igual a la de origen (" + uc.Ubicacion + ")"
                                };
                            }
                            uc.IdUbicacion = BodegaBusiness.GetIdUbicacionByName(uc.Ubicacion);
                            lote.Cantidad += uc.Cantidad;
                        }
                        line.Cantidad+=lote.Cantidad;
                    }
                }
                var ms = ProcessTS(me);
                if (string.IsNullOrEmpty(ms.Error)) {
                    ms.DocNum = GetDocNumBYId(ms.Id);
                }
                return ms;
            }
            finally{
                Monitor.Exit(control);
            }
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

        private static void SetLotesFEFO(TsBodegaLineaMsg line, string CodBodegaDesde)
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
            
        }

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
                    return sapTransferenciaStock.Add(me);
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
