using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR.Client;


namespace jbp.businessServices.SendFacturasAndNCPromotick
{
    static class Program
    {
         
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            //Init();
            //SubscribeOrdersFromRemoteClients();
        }

        //desde la aplicacion jbp de angular se manda las ordenes de start y stop
        private static void SubscribeOrdersFromRemoteClients()
        {
            var hubConnection = new HubConnectionBuilder()
                .WithUrl(conf.Default.urlHub)
                .Build();

            //cuando pierde conexión se vuelve a conectar
            hubConnection.Closed += async (error) =>
                await hubConnection.StartAsync();
            hubConnection.On("start",()=>StartService());
            hubConnection.On("stop", () => StopService());
        }

     

        private static void Init()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new EnvioFacturas_y_NC()
            };
            ServiceBase.Run(ServicesToRun);
        }
        private static void StopService()
        {
            var service = GetService();
            if (service.Status == ServiceControllerStatus.Running) {
                service.Stop();
            }
        }
        private static void StartService()
        {
            var service = GetService();
            try
            {
                if (service.Status == ServiceControllerStatus.Stopped ||
                    service.Status == ServiceControllerStatus.Paused)
                {
                    // se espera máximo un minuto para que inicie el servicio
                    TimeSpan timeout = TimeSpan.FromMilliseconds(60 * 1000);
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                }
            }
            catch (Exception e)
            {
                var error = e.Message;
            }
        }
        private static ServiceController GetService()
        {
            return new ServiceController(new ProjectInstaller().ServiceName);
        }
        private static void test()
        {
            var date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
