using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Timers;
using TechTools.DelegatesAndEnums;
using TechTools.Utils;
using TechTools.Msg;

namespace jbp.business.services
{
    public abstract class BaseServiceTimer: BaseTimer
    {
        private string serviceName;
        public BaseServiceTimer(string serviceName){
            this.serviceName = serviceName;
        }
        
        /// <summary>
        /// Inicia el servicio
        /// </summary>
        /// <param name="loopOnSeconds">Intervalo en segundos en que se ejecuta el método Process</param>
        public override void Start(long loopOnSeconds) {
            if (!IsRunning())
            {
                Log(eTypeLog.Info, "Iniciando servicio ...");
                base.Start(loopOnSeconds);
                GetStatus();
            }
        }
        /// <summary>
        /// corre el servicio a una hora dada en formato de 24 horas
        /// Al correr esta función no hace falta correr la función Start()
        /// </summary>
        public override void StartAt(InitAt initAt) {
            base.StartAt(initAt);
            GetStatus();
        }
        
        /// <summary>
        /// Para el servicio
        /// </summary>
        public override void Stop() {
            if (IsRunning())
            {
                Log(eTypeLog.Info, "Parando servicio...");
                base.Stop();
                GetStatus();
            }
        }
        public override bool IsRunning() {
            var isRunning= base.IsRunning();
            Log(isRunning ? eTypeLog.Info : eTypeLog.Advertencia, 
                string.Format("is runing: {0}",isRunning.ToString().ToLower()));
            return isRunning;
        }
        /// <summary>
        /// El proceso que va a correr cada cierto tiempo
        /// </summary>
        
        public void Log(eTypeLog typeLog, string msg) {
            var log = new LogMsg { type = typeLog, msg = msg };
            LogUtils.AddLog(log);
            NotifyEventToClients(typeLog, log);
        }
        /// <summary>
        /// este metodo se comunica con signal R para notificar a los clientes
        /// </summary>
        /// <param name="tipo"></param>
        /// <param name="msg"></param>
        private void NotifyEventToClients(eTypeLog tipo, LogMsg me)
        {
            var url = config.Default.urlNotificationClient;
            var rc = new RestCall();
            //No se llama al método asíncrono porque da error en la capa jbp.services.rest
            rc.SendPostOrPut(url, typeof(string), me, typeof(LogMsg), RestCall.eRestMethod.POST);
            if (!string.IsNullOrEmpty(rc.ErrorMessage))
                LogUtils.AddLog(new LogMsg {
                    date =me.date, type = eTypeLog.Error, msg = rc.ErrorMessage });
        }
        public virtual string GetStatus() {
            Log(eTypeLog.Info, "Consultado estado del servicio...");
            var ms = string.Format("Status: El servicio de {0} ", this.serviceName);
            if (IsRunning())
            {
                ms += "está corriendo";
                if (this.initAt != null)
                    ms = string.Format("{0} todos los días a las {1} horas con {2} minutos",
                        ms, this.initAt.Hour, this.initAt.Minute);
                else
                    ms = string.Format("{0} cada {1} minutos", ms, this.loopOnSeconds);
            }
            else
                ms += "está Parado";
            Log(eTypeLog.Info, ms);
            return ms;
        }
    }
}
