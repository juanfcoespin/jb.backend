using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using jbp.proxy;
using jbp.msg;
using System.Collections.Generic;

namespace jbp.unitTest.proxy
{
    [TestClass]
    public class FacturaProxyUT
    {
        [TestMethod]
        public void GetFacturasAEnviarATrandinaUT()
        {
            var fp = new FacturaProxy();
            var ms = fp.GetFacturasAEnviarATrandina();
            Assert.AreEqual(30, ms.Count);
        }
    }
}
