using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using jbp.msg.sap;
using jbp.business.hana;

namespace jbp.unitTest.business.hana
{
    [TestClass]
    public class OrdenBusinessUT
    {
        [TestMethod]
        public void DuplicateOrder()
        {
            var order = new OrdenMsg();
            order.CodCliente = "C1713183018001";
            order.Lines.Add(
                new OrdenLinesMsg
                {
                    CodArticulo= "80000038",
                    CantSolicitada=50,
                    CantBonificacion=10
                }
            );
            /*order.Lines.Add(
                new OrdenLinesMsg
                {
                    CodArticulo = "80000057",
                    CantSolicitada = 12,
                    CantBonificacion = 3
                }
            );
            order.Lines.Add(
                new OrdenLinesMsg
                {
                    CodArticulo = "80000110",
                    CantSolicitada = 12,
                    CantBonificacion = 3
                }
            );
            order.Lines.Add(
                new OrdenLinesMsg
                {
                    CodArticulo = "80000007",
                    CantSolicitada = 12,
                    CantBonificacion = 3
                }
            );*/
            var existOrderOnBdd = new OrderBusiness().DuplicateOrder(order);
            //pasa la prueba cuando no hay una orden anterior registrada en la bdd en este dia
            Assert.AreEqual(existOrderOnBdd, false);
        }
    }
}
