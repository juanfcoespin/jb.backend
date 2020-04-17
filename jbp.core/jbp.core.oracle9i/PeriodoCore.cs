using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using jbp.msg;
using System.Data;
using TechTools.Core.Oracle9i;

namespace jbp.core.oracle9i
{
    public class PeriodoCore : BaseCore
    {
        public PeriodoMsg GetById(int id)
        {
            var sql = string.Format(@"
                SELECT 
                 OBJECTID CODPERIODO, 
                 PERIODCODE PERIODO, 
                 BEGENDSTART INICIO,
                 BEGENDEND FIN
                FROM
                 FGCALPERIOD CP
                WHERE
                 OBJECTID = {0}
                 AND CP.FINCALID = 20 AND CP.PERIODTYPE = 1", id);
            var dt = GetDataTableByQuery(sql);
            return new PeriodoMsg {
                CodPeriodo = GetInt(dt.Rows[0]["CODPERIODO"]),
                Nombre= dt.Rows[0]["PERIODO"].ToString(),
                FechaInicio=GetDateTime(dt.Rows[0]["INICIO"]),
                FechaFin = GetDateTime(dt.Rows[0]["FIN"])
            };
        }

        public ListMS<ItemCombo> GetList()
        {
            var ms = new ListMS<ItemCombo>();
            var sql = @"
                select objectId id, periodcode periodo from FGCALPERIOD
            ";
            var dt = GetDataTableByQuery(sql);
            foreach (DataRow dr in dt.Rows) {
                ms.List.Add(new ItemCombo {
                    Id = GetInt(dr["id"]),
                    Nombre = dr["periodo"].ToString()
                });
            }
            return ms;
        }
    }
}
