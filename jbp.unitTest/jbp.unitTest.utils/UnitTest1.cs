using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace jbp.unitTest.utils
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var testDate = new DateTime(2020, 2, 25,12,25,50);
            var ms = testDate.ToString("dd/MM/yyyy");
            Assert.AreEqual("25/02/2020", ms);
        }
    }
}
