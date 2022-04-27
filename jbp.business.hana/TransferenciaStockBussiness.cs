using jbp.msg.sap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jbp.core.sapDiApi;
using System.Threading;
using TechTools.Core.Hana;

namespace jbp.business.hana
{
    public class TransferenciaStockBussiness
    {
        public static readonly object control = new object();
        public static SapTransferenciaStock sapTransferenciaStock = new SapTransferenciaStock();

        public static string SaveFromOF(SalidaBodegaMsg me)
        {
            Monitor.Enter(control);
            try
            {
                var ms = ProcessTSFromOF(me);
                return ms;
            }
            finally
            {
                Monitor.Exit(control);
            }
        }
        private static string ProcessTSFromOF(SalidaBodegaMsg me)
        {
            try
            {
                if (me != null && me.DocNum>0)
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
                        return sapTransferenciaStock.Add(me);
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
    }
    
        
}
