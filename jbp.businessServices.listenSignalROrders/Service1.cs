using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR.Client;
using TechTools.Utils;

namespace jbp.businessServices.listenSignalROrders
{
    public partial class Service1 : ServiceBase
    {
        private static HubConnection hubConnection;
        public static WindowsServiceUtils envioFactAndNcWinService;
        public Service1()
        {
            InitializeComponent();
            envioFactAndNcWinService = new WindowsServiceUtils(conf.Default.envioFacturasNCServiceName);
            SubscribeOrdersFromRemoteClients().Wait();
        }
        //desde la aplicacion jbp de angular se manda las ordenes de start y stop
        private static async Task SubscribeOrdersFromRemoteClients()
        {
            hubConnection = new HubConnectionBuilder()
                .WithUrl(conf.Default.urlHub)
                .Build();
            hubConnection.On("Start", () => envioFactAndNcWinService.StartService());
            hubConnection.On("Stop", () => envioFactAndNcWinService.StopService());

            await hubConnection.StartAsync();

            //cuando pierde conexión se vuelve a conectar
            hubConnection.Closed += async (error) =>
                await hubConnection.StartAsync();

        }
        protected override void OnStart(string[] args)
        {
        }

        protected override void OnStop()
        {
        }
    }
}
