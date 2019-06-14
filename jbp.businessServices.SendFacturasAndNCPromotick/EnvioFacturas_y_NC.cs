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
using Microsoft.AspNetCore.SignalR.Client;
using TechTools.Utils;

namespace jbp.businessServices.SendFacturasAndNCPromotick
{
    public partial class EnvioFacturas_y_NC : ServiceBase
    {
        private HubConnection hubConnection;
        public CheckFacturasToSendPtkBusinessService businessService;
        public EnvioFacturas_y_NC()
        {
            InitializeComponent();
            this.businessService = new CheckFacturasToSendPtkBusinessService();
            SubscribeOrdersFromRemoteClients().Wait();

        }
        private  async Task SubscribeOrdersFromRemoteClients()
        {
            hubConnection = new HubConnectionBuilder()
                .WithUrl(conf.Default.urlHub)
                .Build();
            hubConnection.On("Status", () => SendStatus());
            await hubConnection.StartAsync();

            //cuando pierde conexión se vuelve a conectar
            hubConnection.Closed += async (error) =>
                await hubConnection.StartAsync();

        }
        private void SendStatus()
        {
            //envia el status como log que siempre esta escuchando el cliente
            this.businessService.GetStatus();
            
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
