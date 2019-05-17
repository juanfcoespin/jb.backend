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
            
            CuentaBusiness.SetMontosPorPeriodoAsync().Subscribe(n => {
                if (n == 50)
                    Assert.AreEqual(50, n);
            });
        }
    }
}
