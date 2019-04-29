using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using utilities;

namespace jbp.utils.test
{
    [TestClass]
    public class StringUtilsTest
    {
        [TestMethod]
        public void getTwoDigitNumberTest()
        {
            Assert.AreEqual(StringUtils.getTwoDigitNumber(5), "05");
            Assert.AreEqual(StringUtils.getTwoDigitNumber(30), "30");
        }
        [TestMethod]
        public void QuitarTabTest()
        {
            var test = "here is a string    with a tab    and      spaces";
            var spected= "here is a string with a tab and spaces";
            Assert.AreEqual(Cadena.QuitarTabs(test), spected);
        }
    }
}
