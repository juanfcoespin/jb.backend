using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComunDelegates;
using jbp.business.observables;
using jbp.business.observers;

namespace jbp.business.services
{
    public class CheckFacturasToSendPtkBusinessService : BaseServiceTimer,contracts.INotificationLog
    {
        public  event dLogNotification LogEvent;

        public override void Start(long loopOnSeconds)
        {
            this.LogNotificationEvent += LogEvent;
            base.Start(loopOnSeconds);
        }
        
        public override void Process()
        {
            LogEvent?.Invoke(eTypeLog.Info, "Inciciando consulta de facturas por enviar");
            var facturaPtkBusiness = new FacturaPromotickBusiness();
            facturaPtkBusiness.LogNotificationEvent += LogEvent;
            var facturasPorProcesar = facturaPtkBusiness.GetCurrentMonthFacturasToSendWS();
            
            if(facturasPorProcesar!=null && facturasPorProcesar.Count > 0)
            {
                //facturaPtkBusiness.SendFacturaToWsAsync(facturasPorProcesar);
                facturaPtkBusiness.SendFacturasByFtpAsync(facturasPorProcesar);
                facturaPtkBusiness.InsertFacturasEnviadasAPromotick(facturasPorProcesar);
            }
            
        }
    }
}
