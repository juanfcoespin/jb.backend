using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace jbp.utils.test
{
    [TestClass]
    public class FechaUtilsTest
    {
        [TestMethod]
        public void getStringDateTest()
        {
            var testDate = DateTime.Now;
            var year = testDate.Year.ToString();
            var month = testDate.Month;
            var day = testDate.Day;
            var stringDate = string.Format("{0}-{1}-{2}", StringUtils.getTwoDigitNumber(day), StringUtils.getTwoDigitNumber(month), year);
            Assert.AreEqual(FechaUtils.getStringDate(testDate, "dd-mm-yyyy"), stringDate);
            stringDate = string.Format("{0}/{1}/{2}", year, StringUtils.getTwoDigitNumber(month), StringUtils.getTwoDigitNumber(day));
            Assert.AreEqual(FechaUtils.getStringDate(testDate, "yyyy/mm/dd"),stringDate );
        }
    }
}
