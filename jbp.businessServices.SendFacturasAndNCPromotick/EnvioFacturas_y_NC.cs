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
using TechTools.Msg;
using jbp.msg;

namespace jbp.businessServices.SendFacturasAndNCPromotick
{
    public partial class EnvioFacturas_y_NC : ServiceBase
    {
        private HubConnection hubConnection;
        public CheckFacturasToSendPtkBusinessService businessService;
        private BaseTimer checkConnectionTimer;
        public EnvioFacturas_y_NC()
        {
            InitializeComponent();
            //System.Diagnostics.Debugger.Launch();
            this.businessService = new CheckFacturasToSendPtkBusinessService();
            SubscribeOrdersFromRemoteClients().Wait();
            InitCheckConnection();

        }
        private void InitCheckConnection()
        {
            this.checkConnectionTimer = new BaseTimer();
            this.checkConnectionTimer.ProcessEvent += CheckConnectionTimer_ProcessEvent;
        }

        private void CheckConnectionTimer_ProcessEvent()
        {
            if (this.hubConnection.State == HubConnectionState.Disconnected)
                this.hubConnection.StartAsync().Wait();
        }
        private  async Task SubscribeOrdersFromRemoteClients()
        {
            hubConnection = new HubConnectionBuilder()
                .WithUrl(conf.Default.urlHub)
                .Build();
            hubConnection.On("Status", () => SendStatus());
            hubConnection.On("RequestTodayLogs", () => SendTodayLogs());
            await hubConnection.StartAsync();

            //cuando pierde conexión se vuelve a conectar
            hubConnection.Closed += async (error) =>
                await hubConnection.StartAsync();

        }

        private void SendTodayLogs()
        {
            var logs = this.businessService.GetTodayLogs();
            var rc = new RestCall();
            var url = conf.Default.urlService + "/responseTodayLogs";
            rc.DataArrived += Rc_DataArrived;
            rc.SendPostOrPutAsync(url, typeof(void), logs, typeof(List<LogMsg>), RestCall.eRestMethod.POST);
        }

        private void Rc_DataArrived(object result, string errorMessage)
        {
            var tmp = "hola";
        }

        private void SendStatus()
        {
            //envia el status como log que siempre esta escuchando el cliente
            this.businessService.GetStatus();
            
        }
        protected override void OnStart(string[] args)
        {
            //System.Diagnostics.Debugger.Launch();
            this.businessService.Start();
            if (this.checkConnectionTimer != null)
                this.checkConnectionTimer.Start(10);
        }
        protected override void OnStop()
        {
            this.businessService.Stop();
            if (this.checkConnectionTimer != null)
                this.checkConnectionTimer.Stop();
        }
    }
}
