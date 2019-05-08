using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using jbp.business;

namespace jbp.unitTest.business
{
    [TestClass]
    public class CuentaUT
    {
        [TestMethod]
        public void SetMontosPorPeriodo()
        {
            var ms= CuentaBusiness.SetMontosPorPeriodo(0);
            var percentageAdvance = 0;
            while (percentageAdvance <= 100)
            {
                percentageAdvance = commonBusiness.GetAdvanceProcessById(ms.Id);
            }
            Assert.AreEqual(100, percentageAdvance);
        }
    }
}
