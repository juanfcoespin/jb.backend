using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using jbp.business;
using jbp.msg;

namespace jbp.unitTest.Core
{
    [TestClass]
    public class FacturaResumenUT
    {
        [TestMethod]
        public void GetListFacturaResumen()
        {
            var sql = "select * from JBPVW_FACTURA_RESUMEN where id=613191";
            var listFaturaResumen = FacturaBusiness.GetListFacturaResumen(sql);
            Assert.AreEqual("1791302400001", listFaturaResumen[0].RucSocioNegocio);
        }
    }
}
