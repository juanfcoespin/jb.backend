using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jbp.msg;

namespace jbp.core
{
    public class PeridoCore : BaseCore
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
    }
}
