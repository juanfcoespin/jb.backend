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
        
        

        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new EnvioFacturas_y_NC()
            };
            ServiceBase.Run(ServicesToRun);
        }
        
        






    }
}
