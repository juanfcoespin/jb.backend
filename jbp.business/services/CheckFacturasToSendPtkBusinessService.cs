using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTools.DelegatesAndEnums;
using jbp.business.observables;
using jbp.business.observers;

namespace jbp.business.services
{
    public class CheckFacturasToSendPtkBusinessService : BaseServiceTimer
    {

        public CheckFacturasToSendPtkBusinessService():base("Envío de Facturas y NC Promotick"){}
        public void Start()
        {
            if (config.Default.ptkIniciarAHoraEspesifica)
            {
                var init = new InitAt {
                    Hour =config.Default.ptkIniciarA_Hora,
                    Minute=config.Default.ptkIniciarA_Minuto
                };
                base.StartAt(init);
            }else
                base.Start(config.Default.ptkPeriodoEnSegundosDeConsultaServicio);
        }
        public override void Process()
        {
            Log(eTypeLog.Info, "Iniciciando consulta de facturas por enviar");
            var facturaPtkBusiness = new FacturaPromotickBusiness();
            facturaPtkBusiness.LogNotificationEvent += (type,msg)=>Log(type,msg);
            var facturasPorProcesar = facturaPtkBusiness.GetCurrentMonthFacturasToSendWS();
            
            if(facturasPorProcesar!=null && facturasPorProcesar.Count > 0)
            {
                // descomentar para que funcione el servicio web
                //facturaPtkBusiness.SendFacturaToWsAsync(facturasPorProcesar);
                facturaPtkBusiness.SendFacturasByFtpAsync(facturasPorProcesar);
                facturaPtkBusiness.InsertFacturasEnviadasAPromotick(facturasPorProcesar);
            }
            
        }
    }
}
