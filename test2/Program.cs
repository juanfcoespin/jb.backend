using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using jbp.msg;
using Microsoft.AspNetCore.SignalR.Client;
using TechTools.Msg;
using TechTools.Utils;

namespace test2
{
    class Program
    {
        private static HubConnection hubConnection;
        static void Main(string[] args)
        {
            testSignalRClient();
            Console.ReadLine();
        }

        private static async Task testSignalRClient()
        {
            var serviceName = "CscService";
            var winService = new WindowsServiceUtils(serviceName);
            hubConnection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/checkOrdersToPromotickBusinessService")
                .Build();

            hubConnection.On("Start", () => {
                var ms = winService.StartService();
                if ( ms== "ok")
                    Console.WriteLine("started");
                else
                    Console.WriteLine("not started: "+ms);
            });
            hubConnection.On("Stop", () => {
                var ms = winService.StopService();
                if (ms=="ok")
                    Console.WriteLine("stoped");
                else
                    Console.WriteLine("not stoped: "+ms);
            });
            await hubConnection.StartAsync();

            //cuando pierde conexión se vuelve a conectar
            hubConnection.Closed += async (error) =>
                await hubConnection.StartAsync();
            
            
            
        }
        private static void showLog(LogMsg log) {
            Console.WriteLine(log.msg);
        }

        private static void TestSendMessageSignalR()
        {
            var opt = 0;
            while (opt != 2) {
                PrintMenu();
                opt = Convert.ToInt32(Console.ReadLine());
                switch (opt) {
                    case 1://sendMessage
                        Console.WriteLine("mensaje: ");
                        var msg = Console.ReadLine();
                        SendMsg(msg);
                        break;
                }
            }
            
        }

        private static void SendMsg(string msg)
        {
            var url = "http://localhost:5000/api/message";
            //var rc = new RestCall();
        }

        private static void PrintMenu()
        {
            var msg = @"
1. Send Message
2. Salir

Opt: ";
            Console.WriteLine(msg);
        }

        private static void UploadFileByFtpTest()
        {
            //var cred = new FtpUtils.Credencials {
            //    Url = "ftp://ftp.jbp.com.ec/promotick",
            //    User = "jespin",
            //    Pwd = "2816Jfen*"
            //};
            //FtpUtils.UploadFile(cred, @"c:\temp\tmp.PDF");
        }

        //private static void ExecelUtilsTest()
        //{
        //    var facturas = GetListFacturas();
        //    var ex = new ExcelUtils("Test.xlsx");
        //    ex.AddColumn("Fecha");
        //    ex.AddColumn("Nro Documento");
        //    ex.AddColumn("Monto");
        //    ex.AddColumn("Puntos");
        //    ex.AddColumn("Nro Factura");
        //    var row = 1;
        //    facturas.ForEach(factura => {
        //        ex.AddData(row, 0, factura.fechaFactura);
        //        ex.AddData(row, 1, factura.numDocumento);
        //        ex.AddData(row, 2, factura.montoFactura);
        //        ex.AddData(row, 3, factura.puntos);
        //        ex.AddData(row, 4, factura.numFactura);
        //        row++;
        //    });
        //    ex.EndEditAndSave();
        //}
        private static List<FacturaPromotickMsg> GetListFacturas()
        {
            var ms = new List<FacturaPromotickMsg>();
            ms.Add(
                new FacturaPromotickMsg {
                    fechaFactura = "14/05/2019",
                    numDocumento = "0303281631",
                    montoFactura = 324,
                    puntos=778,
                    numFactura= "001-010-0002345"
                }
            );
            ms.Add(
                new FacturaPromotickMsg
                {
                    fechaFactura = "15/05/2019",
                    numDocumento = "1712643608",
                    montoFactura = 325,
                    puntos = 878,
                    numFactura = "001-010-0002346"
                }
            );
            ms.Add(
                new FacturaPromotickMsg
                {
                    fechaFactura = "16/05/2019",
                    numDocumento = "0203281649001",
                    montoFactura = 326,
                    puntos = 978,
                    numFactura = "001-010-0002347"
                }
            );
            ms.Add(
                new FacturaPromotickMsg
                {
                    fechaFactura = "16/05/2019",
                    numDocumento = "0903281649001",
                    montoFactura = 326,
                    puntos = 978,
                    numFactura = "001-010-0002347"
                }
            );
            return ms;
        }
    }
}
