using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using jbp.msg;
using TechTools.Core.Hana;
using System.Data;
using jbp.msg.sap;


namespace jbp.business.hana
{
    public class BodegaBusiness
    {
        public static List<SubNivelBodegaMsg> GetSubnivelesAlmacen()
        {
            var ms = new List<SubNivelBodegaMsg>();
            var sql = @"
               select
                ""Id"",
                ""Codigo"", 
                ""Descripcion""
               from ""JbpVw_SubNivelesAlmacen""
               where 
                ""Descripcion"" is not null
                and ""Codigo"" not like '%SYSTEM%'
                and (
                    ""Descripcion"" like 'PERCHA%'
                    OR ""Descripcion"" like 'NIVEL%'
                    OR ""Descripcion"" like 'SECCION%'
                    OR ""Descripcion"" like 'UBICACIÓN%'
                )
              order by
                ""Codigo""
            ";
            var bc = new BaseCore();
            var dt = bc.GetDataTableByQuery(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows){
                    ms.Add(new SubNivelBodegaMsg{
                        id = bc.GetInt(dr["ID"]),
                        codigo = dr["Codigo"].ToString(),
                        descripcion = dr["Descripcion"].ToString()
                    });
                }
            }
            return ms;
        }
    }
}
