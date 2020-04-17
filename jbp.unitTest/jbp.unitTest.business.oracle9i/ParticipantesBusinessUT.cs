using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using jbp.business.oracle9i.promotick;
using jbp.msg;
using System.Linq;

namespace jbp.unitTest.business.oracle9i
{
    [TestClass]
    public class ParticipantesBusinessUT
    {
        [TestMethod]
        public void GetParticipantesTest()
        {
            var participantes = new ParticipantesBusiness().GetParticipantes();
            var participantesConSucursales = participantes.Where(p=>p.RucsSecundarios.Count>2).ToList();
            Assert.AreEqual(true, participantes.Count>0);//si se consultan participantes de la bdd oracle
        }
    }
}
