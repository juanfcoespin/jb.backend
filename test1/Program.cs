using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTools.Core.Oracle9i;
using jbp.business.oracle9i;
using TechTools.Core.Hana;
using jbp.business.hana;
using System.Data;


namespace test1
{
    class Program
    {
        static void Main(string[] args)
        {
            //var sql = "select * from JBPVW_SOCIONEGOCIO where rownum<5";
            //var dt = new TechTools.Core.Oracle9i.BaseCore().GetDataTableByQuery(sql);
            //var ms = dt.Rows.Count;
            //sql = "select top 10 * from OINV";
            //var dt2 = new TechTools.Core.Hana.BaseCore().GetDataTableByQuery(sql);
            //ms = dt2.Rows.Count;
            
            var ms = jbp.business.oracle9i.SocioNegocioBusiness.GetItemsBytoken("18032816");
            var ms2 = jbp.business.hana.SocioNegocioBusiness.GetParticipantePuntosByRuc("1802919496001");

        }
    }
}
