using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using jbp.msg;
using TechTools.Core.Hana;
using System.Data;

namespace jbp.business.hana
{
    public class ConfBusiness
    {
        public static int GetNumHojaRuta()
        {
            var sql = "SELECT top 1 NUM_HOJA_RUTA FROM JBP_CONF";
            var ms = new BaseCore().GetIntScalarByQuery(sql);
            return ms;
        }
    }
}
