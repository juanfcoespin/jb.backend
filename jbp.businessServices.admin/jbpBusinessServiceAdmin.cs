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
using TechTools.DelegatesAndEnums;

namespace jbp.businessServices.admin
{
    public partial class jbpBusinessServiceAdmin : ServiceBase
    {
        private HubConnection hubConnection;
        public WindowsServiceUtils envioFactAndNcWinService;
        private BaseTimer checkConnectionTimer;

        public jbpBusinessServiceAdmin()
        {
            InitializeComponent();
            
            envioFactAndNcWinService = new WindowsServiceUtils(conf.Default.envioFacturasNCServiceName);
            SubscribeOrdersFromRemoteClients().Wait();
            InitCheckConnection();

        }
        private void InitCheckConnection() {
            this.checkConnectionTimer = new BaseTimer();
            this.checkConnectionTimer.ProcessEvent += CheckConnectionTimer_ProcessEvent;
        }

        private void CheckConnectionTimer_ProcessEvent()
        {
            if (this.hubConnection.State == HubConnectionState.Disconnected)
                this.hubConnection.StartAsync().Wait();
        }

        //desde la aplicacion jbp de angular se manda las ordenes de start y stop
        private async Task SubscribeOrdersFromRemoteClients()
        {
            hubConnection = new HubConnectionBuilder()
                .WithUrl(conf.Default.promotickServiceHubUrl)
                .Build();
            hubConnection.On("Start", () => this.envioFactAndNcWinService.StartService());
            hubConnection.On("Stop", () => this.envioFactAndNcWinService.StopService());
            hubConnection.On("CheckIsRunning", () => SendIsRunningResponse());
            await hubConnection.StartAsync();

            //cuando pierde conexión se vuelve a conectar
            hubConnection.Closed += async (error) =>
                await hubConnection.StartAsync();

        }

        private void SendIsRunningResponse()
        {
            bool isRunning = this.envioFactAndNcWinService.IsRunning();
            var restCall = new RestCall();
            var url = conf.Default.promotickBusinessServiceOrdersUrl + "/isRunningResponse";
            restCall.SendGetAsync(url + "/" + isRunning.ToString(), typeof(void));
        }

        protected override void OnStart(string[] args)
        {
            if (this.checkConnectionTimer == null)
                InitCheckConnection();
            this.checkConnectionTimer.Start(10);
        }

        protected override void OnStop()
        {
            if (this.checkConnectionTimer != null)
                this.checkConnectionTimer.Stop();
        }
    }
}
