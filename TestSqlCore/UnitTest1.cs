using NUnit.Framework;
using jbp.core.sqlExpress;

namespace TestSqlCore
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var sqlCon = new SqlExpress();
            Assert.AreEqual(sqlCon.Connect(), "ok");
        }
    }
}