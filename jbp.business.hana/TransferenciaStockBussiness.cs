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
        public static string Transfer(TsBodegaMsg me)
        {
            Monitor.Enter(control);
            try
            {
                SetLotesFEFO(me);
                //SetSerie(me); //La serie del documento
                var ms = ProcessTS(me);
                return ms;
            }
            finally
            {
                Monitor.Exit(control);
            }
        }

        //private static void SetSerie(TsBodegaMsg me)
        //{
        //    if (me.CodBodegaHasta.ToUpper().Contains("1")) // Ej: MAT1, PSJ1, PROD1
        //        me.Serie = "TR_HUM";
        //    if (me.CodBodegaHasta.ToUpper().Contains("2")) // Ej: MAT2, PSJ2, PROD2
        //        me.Serie = "TR_VET";
        //    if (me.CodBodegaHasta.ToUpper().Contains("CUAR")) // A CUARENTENA
        //        me.Serie = "TR_CC";
        //}

        private static void SetLotesFEFO(TsBodegaMsg me)
        {
            me.Lineas.ForEach(line =>
            {
                var cantlotes = GetLotesByCodArt_CodBodega(line.CodArticulo, me.CodBodegaDesde);
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
                        line.Lotes.Add(new LoteEscogidoMsg{
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
            });
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

        private static string ProcessTS(TsBodegaMsg me)
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
                return e.Message;
            }
        }
       
    }
    
        
}
