using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using jbp.business.hana;    

namespace jbp.unitTest.business.hana
{
    [TestClass]
    public class TestCorreo
    {
        [TestMethod]
        public void TestMail()
        {
            var bb=new BaseBusiness();
            var resp=bb.EnviarPorCorreo("jespin@jbp.com.ec", "test", "Esta es una prueba unitaria de correo");
            
            Assert.AreEqual(resp, true);
        }
    }
}
