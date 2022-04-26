using jbp.msg.sap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jbp.core.sapDiApi;
using System.Threading;

namespace jbp.business.hana
{
    public class EmisionProduccionBussiness
    {
        public static readonly object control = new object();
        public static SapEmisionProduccion sapEmisionProduccion = new SapEmisionProduccion();

        public static string Save(SalidaBodegaMsg me)
        {
            Monitor.Enter(control);
            try
            {
                var ms = ProcessEP(me);
                return ms;
            }
            finally
            {
                Monitor.Exit(control);
            }
        }
        private static string ProcessEP(SalidaBodegaMsg me)
        {
            try
            {
                if (me != null && me.DocNum>0)
                {
                    if (me.DocBaseType == EDocBase.OrdenFabricacion) {
                        me.IdDocBase = OrdenFabricacionBusiness.GetIdByDocNum(me.DocNum);
                        if (!OrdenFabricacionBusiness.EstaLiberada(me.IdDocBase))
                            return String.Format("La orden de fabricacion: {0} no está en estado liberada!!", me.DocNum);
                        if (sapEmisionProduccion == null)
                            sapEmisionProduccion = new SapEmisionProduccion();
                        if (!sapEmisionProduccion.IsConected())
                            sapEmisionProduccion.Connect();//se conecta a sap
                        return sapEmisionProduccion.Add(me);
                    }
                    
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
