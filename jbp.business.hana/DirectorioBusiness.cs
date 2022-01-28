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
    public class DirectorioBusiness
    {
        public static List<ContactoMsg> GetDirectorio()
        {
            var ms = new List<ContactoMsg>();
            var sql = @"
            select 
             CONTACTO,
             DEPARTAMENTO,
             PLANTA,
             EXT
            from
             JBP_DIRECTORIO_TELEFONICO
            ";
            var dt = new BaseCore().GetDataTableByQuery(sql);
            foreach(DataRow dr in dt.Rows)
            {
                ms.Add(
                new ContactoMsg
                {
                    CONTACTO = dr["CONTACTO"].ToString(),
                    DEPARTAMENTO = dr["DEPARTAMENTO"].ToString(),
                    PLANTA = dr["PLANTA"].ToString(),
                    Ext = dr["EXT"].ToString()
                }
            );
            }
            return ms;
        }
    }
}
