using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTools.Core.Hana;

namespace jbp.business.hana
{
    public class EmpleadoBusiness
    {
        public static string GetMailByCedula(string cedula)
        {
            var sql = string.Format(@"
                select ""U_idemail"" 
                from ""@A1A_MAFU"" 
                where ""U_idcodigo""='{0}'
            ",cedula);
            return new BaseCore().GetScalarByQuery(sql);
        }
    }
}
