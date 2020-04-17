using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using jbp.business.hana;
using jbp.msg;

namespace jbp.unitTest.business.hana
{
    [TestClass]
    public class FacturaBusinessUT
    {
        [TestMethod]
        public void GetFacturasToSendPromotick()
        {
            var ms = new FacturaBusiness().GetFacturasParticipantesToSendPromotick();
            Assert.AreEqual(true, ms.Count > 0);
        }
    }
}
