using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jbp.core;

namespace jbp.business
{
    public class NotaCreditoBusiness
    {
        public static void RegistrarAutorizacionSRI(string numAutorizacion, string idDocumento)
        {
            var bc = new BaseCore();
            var sql = string.Format(@"
                UPDATE GMS.TBL_NC_SRI SET AUTSRI = '{0}' WHERE NC = {1}",
                numAutorizacion, idDocumento);
            bc.Execute(sql);
            sql = string.Format(@"
                UPDATE COINVCSUMRY SET CMNT = '{0}' WHERE OBJECTID = {1}",
                numAutorizacion, idDocumento);
            bc.Execute(sql);
        }
    }
}
