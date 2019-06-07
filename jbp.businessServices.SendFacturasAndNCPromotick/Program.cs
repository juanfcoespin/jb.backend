using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace jbp.businessServices.SendFacturasAndNCPromotick
{
    static class Program
    {
         
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            var ptkService = new EnvioFacturas_y_NC();
            //Init(ptkService);
            //test();
            //StartService();
            StopService();
        }
        private static void Init(EnvioFacturas_y_NC ptkService)
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                ptkService
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
