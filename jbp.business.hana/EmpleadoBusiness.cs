using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTools.Core.Hana;
using System.Data;

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

        public static object getCumple()
        {
            try
            {
                var sql = string.Format(@"
                    select 
                     top 3
                     ""U_idprinom""||' '|| ""U_idsegnom"" ||' '|| ""U_idapepat"" ""Nombre"",
                     to_char(""U_idfecnac"", 'yyyy-mm-dd') ""FechaNacimiento"",
                     t1.""U_nombre"" ""Cargo"",
                     t0.""U_idemail"" ""Email""                    
                    from
                     ""@A1A_MAFU"" t0 inner join
                     ""@A1A_TICA"" t1 on t1.""U_codigo"" = t0.""U_uocodcar""
                    where
                     ""U_ctestado"" = 'V'--vigente
                     --and to_char(current_date, 'mm-dd') = to_char(""U_idfecnac"", 'mm-dd')
                     and to_char(current_date, 'mm') = to_char(""U_idfecnac"", 'mm')
                ");
                var dt = new BaseCore().GetDataTableByQuery(sql);
                var ms=new List<object>();  
                foreach (DataRow dr in dt.Rows) {
                    ms.Add(new {
                        Nombre = dr["Nombre"].ToString(),
                        FechaNacimiento = dr["FechaNacimiento"].ToString(),
                        Cargo = dr["Cargo"].ToString(),
                        Email = dr["Email"].ToString(),
                    });
                }
                return new
                {
                    data = ms
                };
            }
            catch (Exception e)
            {

                return new
                {
                    error = "Error: " + e.Message
                };
            }
        }
    }
}
