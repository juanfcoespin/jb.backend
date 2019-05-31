using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Timers;
using TechTools.DelegatesAndEnums;
using TechTools.Utils;
using TechTools.Msg;
using jbp.msg;
namespace jbp.business.services
{
    public abstract class BaseServiceTimer
    {
        private string serviceName;
        private Timer timer;
        private long loopOnSeconds;
        public class InitAt {
            public int Hour { get; set; }
            public int Minute { get; set; }
        }
        private InitAt initAt;
        public BaseServiceTimer(string serviceName) {
            this.timer = new Timer();
            this.serviceName = serviceName;
        }
        ~BaseServiceTimer() {
            if (IsRunning()) {
                Stop();
            }
        }
        /// <summary>
        /// Inicia el servicio
        /// </summary>
        /// <param name="loopOnSeconds">Intervalo en segundos en que se ejecuta el método Process</param>
        public virtual void Start(long loopOnSeconds) {
            if (!IsRunning())
            {
                Log(eTypeLog.Info, "Iniciando servicio ...");
                this.loopOnSeconds = loopOnSeconds;
                this.timer = new Timer(loopOnSeconds * 1000);
                this.timer.Elapsed += Timer_Elapsed;
                this.timer.AutoReset = true;
                this.timer.Enabled = true;
                this.timer.Start();
                GetStatus();
                if (initAt == null)//cuado el servicio se corre de forma forma periódica
                    Process();
            }
        }
        /// <summary>
        /// corre el servicio a una hora dada en formato de 24 horas
        /// Al correr esta función no hace falta correr la función Start()
        /// </summary>
        public void StartAt(InitAt initAt) {
            if (initAt != null) {
                this.initAt = initAt;
            }
            Start(30);//verifica la hora de ejecución del servicio cada 30 seg
            GetStatus();
        }
        private  void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var currenTime = GetCurrentTime();
            if(initAt== null || (initAt.Hour==currenTime.Hour && initAt.Minute==currenTime.Minute))
                Process();
        }
        private InitAt GetCurrentTime()
        {
            return new InitAt
            {
                Hour = DateTime.Now.Hour,
                Minute = DateTime.Now.Minute
            };
        }
        /// <summary>
        /// Para el servicio
        /// </summary>
        public virtual void Stop() {
            Log(eTypeLog.Info, "Parando servicio...");
            this.timer.Stop();
            this.timer.Elapsed -= Timer_Elapsed;
            this.timer.Enabled = false;
            GetStatus();
        }
        public bool IsRunning() {
               return this.timer.Enabled;
        }
        /// <summary>
        /// El proceso que va a correr cada cierto tiempo
        /// </summary>
        public abstract void Process();
        public void Log(eTypeLog typeLog, string msg) {
            LogUtils.AddLog(
                new LogMsg {type =typeLog , msg = msg }
            );
            NotifyEventToClients(typeLog, msg);
        }
        /// <summary>
        /// este metodo se comunica con signal R para notificar a los clientes
        /// </summary>
        /// <param name="tipo"></param>
        /// <param name="msg"></param>
        private void NotifyEventToClients(eTypeLog tipo, string msg)
        {
            var date = DateTime.Now.ToString("yyyy-mm-dd HH:mm:ss");
            var url = config.Default.urlNotificationClient;
            var rc = new RestCall();
            var me = new LogMsg { date=date,type=tipo,msg=msg };
            //No se llama al método asíncrono porque da error en la capa jbp.services.rest
            rc.SendPostOrPut(url, typeof(string), me, typeof(LogMsg), RestCall.eRestMethod.POST);
            if (!string.IsNullOrEmpty(rc.ErrorMessage))
                LogUtils.AddLog(new LogMsg { type = eTypeLog.Error, msg = rc.ErrorMessage });
        }
        public virtual string GetStatus() {
            Log(eTypeLog.Info, "Consultado estado del servicio...");
            var ms = string.Format("El servicio de {0} ", this.serviceName);
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
