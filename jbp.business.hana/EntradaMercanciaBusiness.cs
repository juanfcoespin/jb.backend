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

        public static string IngresarPorCompra(EntradaMercanciaMsg me)
        {
            Monitor.Enter(control);
            try
            {
                var ms = ProcessEMPorCompra(me);
                return ms;
            }
            finally
            {
                Monitor.Exit(control);
            }
        }

        
        private static string ProcessEMPorCompra(EntradaMercanciaMsg me)
        {
            try
            {
                if (me != null)
                {
                    me.CodBodega = "CUAR1"; //por defecto toda compra va a cuarentena
                    if (sapEntradaMercancia == null)
                        sapEntradaMercancia = new SapEntradaMercancia();
                    if (!sapEntradaMercancia.IsConected())
                        sapEntradaMercancia.Connect();//se conecta a sap
                    return sapEntradaMercancia.AddPorCompra(me);
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
