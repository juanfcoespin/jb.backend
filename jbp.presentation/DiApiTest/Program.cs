using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jbp.business.hana;
using jbp.msg.sap;
using TechTools.Rest;

namespace DiApiTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start:");
            Console.WriteLine(DateTime.Now);
            //OrderBusiness.check();
            Console.WriteLine("Stop:");
            Console.WriteLine(DateTime.Now);
            //testEnvio();
        }
        static void testEnvio()
        {
            var me = new List<OrdenMsg>();
            var o1 = new OrdenMsg();
            o1.CodCliente = "C2100051941"; //Gardenia
            o1.Comentario = "Prueba de sistemas no procesar!!!";
            o1.AddLine("80000096", 50, 2); //ivermec 10ml
            me.Add(o1);
            var rc = new RestCall();
            var url = "http://services.jbp.com.ec/api/orden";
            var ms = rc.SendPostOrPut(url, typeof(List<string>), me, typeof(List<OrdenMsg>), RestCall.eRestMethod.POST);
            //var ms= new OrderBusiness().SaveOrders(me);
            //ms.ForEach(item => Console.WriteLine(item));
        }
    }
}
