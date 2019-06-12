using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

using jbp.business.services;

namespace jbp.businessServices.SendFacturasAndNCPromotick
{
    public partial class EnvioFacturas_y_NC : ServiceBase
    {
        
        public CheckFacturasToSendPtkBusinessService businessService;
        
        public EnvioFacturas_y_NC()
        {
            InitializeComponent();
            this.businessService = new CheckFacturasToSendPtkBusinessService();
        }
        

        protected override void OnStart(string[] args)
        {
            this.businessService.Start();
        }

        protected override void OnStop()
        {
            this.businessService.Stop();
        }

    }
}
