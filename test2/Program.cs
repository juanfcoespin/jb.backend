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
            PrintTodayLogs();
            Console.ReadLine();
        }

        private static void PrintTodayLogs()
        {
            var tmp = DateTime.Now.ToString("yyyy-MM-dd");
            var logs=LogUtils.GetLogsByDate(DateTime.Now);
            Console.WriteLine(logs.Count);
        }

        private static void PruebaLlamadaWsPromotick()
        {
            var auth = new RestCall.AuthenticationMe {
                User= "api-james-promotick",
                Pwd= "hkIUtJmnq5sda",
                AuthType=RestCall.eAuthType.Basic
            };
            var url = "http://apijames.promotick.com.pe/api/gsttransaccion";
            var facturas = getFacturasPtk();
            var rc = new RestCall();
            rc.DataArrived += (resp, err) =>
            {
                if (!string.IsNullOrEmpty(err))
                    Console.WriteLine(err);
                else {
                    Console.WriteLine(SerializadorJson.Serializar(resp));
                }
            };
            rc.SendPostOrPutAsync(url, typeof(RespuestasPtkWsFacturasMsg), facturas,
                typeof(FacturasPtkMsg), RestCall.eRestMethod.POST, auth);
        }
        private static FacturasPtkMsg getFacturasPtk()
        {
            var ms = new List<FacturaPromotickMsg>();
            ms.Add(new FacturaPromotickMsg {
                id=21,
                fechaFactura="10/10/2018",
                numFactura="FC0003",
                descripcion="desc01",
                numDocumento= "1103007496001",
                montoFactura=15,
                puntos=6,
                Error="sin error"
            });
            ms.Add(new FacturaPromotickMsg
            {
                id = 21,
                fechaFactura = "10/10/2018",
                numFactura = "FC0004",
                descripcion = "desc02",
                numDocumento = "1103007496001",
                montoFactura = 152,
                puntos = 62,
                Error = "sin error 2"
            });
            return new FacturasPtkMsg { facturas=ms};
        }
        private static string GetBasicAuthTocken(string usr, string pwd)
        {
            var authHeader = Convert.ToBase64String(
                    ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", usr, pwd)));
            return authHeader;
            //request.Headers.Add("Authorization", auth.AuthType.ToString() + " " + authHeader);
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
