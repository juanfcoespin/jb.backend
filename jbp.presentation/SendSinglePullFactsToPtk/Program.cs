using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//using jbp.business;
//using jbp.business.promotick;
using jbp.msg;
using TechTools.DelegatesAndEnums;
using TechTools.Msg;
using TechTools.Logs;
using TechTools.Utils;

namespace SendSinglePullFactsToPtk
{
    class Program
    {
        static void Main(string[] args)
        {
            new LogUtils().AddLog(new LogMsg() { msg = "chao" });
            /*try
            {
                Console.WriteLine("Iniciado Proceso...");
                var facturasToSend = new List<FacturaPromotickMsg>();
                facturasToSend.Add(new FacturaPromotickMsg
                {
                    fechaFactura = "06/11/2019",
                    numFactura = "001-010-0063166",
                    descripcion = "Factura Ventas",
                    numDocumento = "1600218539001",
                    montoFactura = 937,
                    puntos = 3748
                });
                
                var consumoWsPtk = new ConsumoWsPtkBusiness();
                consumoWsPtk.LogNotificationEvent += (typelog, msg) => Log(typelog, msg);
                consumoWsPtk.SendFacturaToWsAsync(new FacturasPtkMsg { facturas = facturasToSend });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
            Console.WriteLine("Presione una tecla para finalizar");
            Console.ReadKey();*/
        }

        private static void Log(eTypeLog typelog, string msg)
        {
            var mensaje = string.Format("{0}: {1}", typelog.ToString(), msg);
            Console.WriteLine(msg);
            new LogUtils().AddLog(new LogMsg() {msg= mensaje });
        }
    }
}
