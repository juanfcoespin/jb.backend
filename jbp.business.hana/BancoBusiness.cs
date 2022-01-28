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
    public class BancoBusiness
    {
        public static List<ItemCombo> GetBancosEcuador()
        {
            var ms = new List<ItemCombo>();
            var sql = @"
              select 
                 ""CodBanco"",
                 ""Banco""
              from ""JbpVw_Banco""
              where ""CodPais"" = 'EC'
            ";
            var bc = new BaseCore();
            var dt = bc.GetDataTableByQuery(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    ms.Add(new ItemCombo
                    {
                        Cod = dr["CodBanco"].ToString(),
                        Nombre = dr["Banco"].ToString(),
                    });
                }
            }
            return ms;
        }

        
    }
}
