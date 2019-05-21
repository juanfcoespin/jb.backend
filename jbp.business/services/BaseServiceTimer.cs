using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using TechTools.DelegatesAndEnums;


namespace jbp.business.services
{
    public abstract class BaseServiceTimer:contracts.INotificationLog
    {
        private Timer _Timer;
        public class InitAt {
            public int Hour { get; set; }
            public int Minute { get; set; }
        }
        private InitAt _InitAt;
        public event dLogNotification LogNotificationEvent;
        public BaseServiceTimer() {
            _Timer = new Timer();
        }
        /// <summary>
        /// Inicia el servicio
        /// </summary>
        /// <param name="loopOnSeconds">Intervalo en segundos en que se ejecuta el método Process</param>
        public virtual void Start(long loopOnSeconds) {
            if (!IsRunning())
            {
                _Timer = new Timer(loopOnSeconds * 1000);
                _Timer.Elapsed += Timer_Elapsed;
                _Timer.AutoReset = true;
                _Timer.Enabled = true;
                _Timer.Start();
                LogNotificationEvent?.Invoke(eTypeLog.Info, "Servicio Iniciado");
                if (_InitAt == null)//cuado el servicio se corre de forma forma periódica
                    Process();
            }
        }
        /// <summary>
        /// corre el servicio a una hora dada en formato de 24 horas
        /// Al correr esta función no hace falta correr la función Start()
        /// </summary>
        public void StartAt(InitAt initAt) {
            if (initAt != null) {
                this._InitAt = initAt;
            }
            Start(30);//verifica la hora de ejecución del servicio cada 30 seg
            var msg = string.Format("El servicio correrá todos los dias a la hora {0}, minuto {1}",
                initAt.Hour, initAt.Minute);
            LogNotificationEvent?.Invoke(eTypeLog.Info, msg);
        }
        private  void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var currenTime = GetCurrentTime();
            if(_InitAt== null || (_InitAt.Hour==currenTime.Hour && _InitAt.Minute==currenTime.Minute))
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
            _Timer.Stop();
            _Timer.Elapsed -= Timer_Elapsed;
            _Timer.Enabled = false;
            LogNotificationEvent?.Invoke(eTypeLog.Info, "Servicio Parado");
        }
        public bool IsRunning() {
               return _Timer.Enabled;
        }
        /// <summary>
        /// El proceso que va a correr cada cierto tiempo
        /// </summary>
        public abstract void Process();
    }
}
