using jbp.msg.sap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbp.business.hana
{
    public class EntradaMercanciaBusiness
    {
        public static string Ingresar(EntradaMercanciaMsg me)
        {
            try
            {
                var ms = "ok EM";
                return ms;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}
