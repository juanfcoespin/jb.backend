using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using jbp.business.hana;

namespace jbp.unitTest.business.hana
{
    [TestClass]
    public class SocioNegocioUT
    {
        [TestMethod]
        public void GetUnSoloCorreoConComaUT()
        {
            var mails = "juan.espi@yahoo.com, jespin@jbp.com.ec";
            Assert.AreEqual("juan.espi@yahoo.com", SocioNegocioBusiness.GetUnSoloCorreo(mails));
        }
        [TestMethod]
        public void GetUnSoloCorreoConPuntoYComaUT()
        {
            var mails = "juan.espi@yahoo.com; jespin@jbp.com.ec";
            Assert.AreEqual("juan.espi@yahoo.com", SocioNegocioBusiness.GetUnSoloCorreo(mails));
        }
        [TestMethod]
        public void GetUnSoloCorreoConEspacioEnBlanco()
        {
            var mails = "juan.espi@yahoo.com jespin@jbp.com.ec";
            Assert.AreEqual("juan.espi@yahoo.com", SocioNegocioBusiness.GetUnSoloCorreo(mails));
        }
        [TestMethod]
        public void GetUnSoloCorreoConAlgunosEspacioEnBlanco()
        {
            var mails = "  juan.espi@yahoo.com   jespin@jbp.com.ec";
            Assert.AreEqual("juan.espi@yahoo.com", SocioNegocioBusiness.GetUnSoloCorreo(mails));
        }
    }
}
