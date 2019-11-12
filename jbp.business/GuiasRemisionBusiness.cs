using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jbp.core;

namespace jbp.business
{
    public class GuiasRemisionBusiness
    {
        public static void RegistrarAutorizacionSRI(string numAutorizacion, string idDocumento)
        {
            var sql = string.Format(@"
                UPDATE GMS.TBL_GUIAS_REMISION SET AUTSRI = '{0}' WHERE NUMGUIA = {1}",
                numAutorizacion, idDocumento);
            new BaseCore().Execute(sql);
        }
    }
}
