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

        public static string Ingresar(EntradaMercanciaMsg me)
        {
            Monitor.Enter(control);
            try
            {
                var ms = ProcessEM(me);
                return ms;
            }
            finally
            {
                Monitor.Exit(control);
            }
        }

        
        private static string ProcessEM(EntradaMercanciaMsg me)
        {
            try
            {
                if (me != null)
                {
                    if (sapEntradaMercancia == null)
                        sapEntradaMercancia = new SapEntradaMercancia();
                    if (!sapEntradaMercancia.IsConected())
                        sapEntradaMercancia.Connect();//se conecta a sap
                    return sapEntradaMercancia.Add(me);
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
