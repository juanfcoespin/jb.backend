using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTools.Core.Hana;
using System.Data;
//using jbp.business.hana;
//using jbp.msg.sap;


namespace TestConnectionSap10
{
    internal class Program
    {
        static void Main(string[] args)
        {
            testCore();

        }
        /*static void testBussines() {
            var ms = OrdenFabricacionBusiness.GetOfLiberadasPesaje();
        }*/

        static void testCore() {
            var bc = new BaseCore();
            var sql = @"
                select top 10 ""ItemCode"" from OITM
            ";
            var dt = bc.GetDataTableByQuery(sql);
            var firstCode = dt.Rows[0][0].ToString();
        }

    }
}
