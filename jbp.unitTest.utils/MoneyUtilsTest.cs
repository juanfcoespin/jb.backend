using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace jbp.utils.test
{
    [TestClass]
    public class MoneyUtilsTest
    {
        [TestMethod]
        public void GetMoneyFormatTest()
        {
            var m1 = jbp.utils.MoneyUtils.GetMoneyFormat("1,573.35");
            var m2 = jbp.utils.MoneyUtils.GetMoneyFormat("1.573,35");
            var m3 = jbp.utils.MoneyUtils.GetMoneyFormat("1.573.35");
            var m4 = jbp.utils.MoneyUtils.GetMoneyFormat("1573.35");
            var m5 = jbp.utils.MoneyUtils.GetMoneyFormat("1,573,35");
            var m6 = jbp.utils.MoneyUtils.GetMoneyFormat("1,573");
            var m7 = jbp.utils.MoneyUtils.GetMoneyFormat("1573");
            var m8 = jbp.utils.MoneyUtils.GetMoneyFormat("0");
            var m9 = jbp.utils.MoneyUtils.GetMoneyFormat("1.573");
            Assert.AreEqual(m1, "1573.35");
            Assert.AreEqual(m2, "1573.35");
            Assert.AreEqual(m3, "1573.35");
            Assert.AreEqual(m4, "1573.35");
            Assert.AreEqual(m5, "1573.35");
            Assert.AreEqual(m6, "1.573");
            Assert.AreEqual(m7, "1573.00");
            Assert.AreEqual(m8, "0.00");
            Assert.AreEqual(m9, "1.573");
        }
    }
}
