using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jbp.msg.sap;
using TechTools.Core.Hana;
using System.Data;

namespace jbp.business.hana
{
    public class LoteBusiness : BaseBusiness
    {
        internal static List<LoteMsg> GetLotesByCodArticulo(string codArticulo)
        {
            var ms = new List<LoteMsg>();
            var sql = string.Format(@"
            select
             ""Lote"",
             ifnull(to_char(""FechaFabricacion"", 'yyyy-mm'), 'No registrado') ""FechaFabricacion"",
             to_char(""FechaVencimiento"", 'yyyy-mm') ""FechaVencimiento""
            from
             ""JbpVw_Lotes""
            where
             ""CodArticulo"" = '{0}'
            ", codArticulo);
            var dt = new BaseCore().GetDataTableByQuery(sql);
            foreach (DataRow dr in dt.Rows) {
                ms.Add(new LoteMsg
                {
                    lote = dr["Lote"].ToString(),
                    fechaFab = dr["FechaFabricacion"].ToString(),
                    fechaVen = dr["FechaVencimiento"].ToString()
                });
            }
            return ms;
        }
    }
}
