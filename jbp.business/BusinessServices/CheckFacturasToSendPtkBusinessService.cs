using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TechTools.DelegatesAndEnums;
using jbp.business.contracts;
using jbp.business.promotick;
using jbp.msg;

namespace jbp.business.services
{
    public class CheckFacturasToSendPtkBusinessService : BaseServiceTimer
    {
        
        private static List<FacturaPromotickMsg> facturasProcesadas;

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
            var facturaPtkBusiness = new FacturaPromotickBusiness();
            facturaPtkBusiness.LogNotificationEvent += (type, msg) => Log(type, msg);
            facturaPtkBusiness.LogNotificationEvent += (type, msg) => Log(type, msg);
            Log(eTypeLog.Info, "Iniciciando consulta de facturas por enviar");
            var facturasPorProcesar = facturaPtkBusiness.GetCurrentMonthFacturasToSendWS();
            //a veces se procesa mas de una vez el mismo mensaje por eso se incorpora el control !SeProceso()
            if(facturasPorProcesar!=null && facturasPorProcesar.Count > 0 && !SeProceso(facturasPorProcesar))
            {
                facturaPtkBusiness.InsertFacturasEnviadasAPromotick(facturasPorProcesar);
                if (config.Default.ptkEnviarPorWS){
                    var consumoWsPtk = new ConsumoWsPtkBusiness();
                    consumoWsPtk.LogNotificationEvent += (typelog, msg) => Log(typelog, msg);
                    consumoWsPtk.SendFacturaToWsAsync(new FacturasPtkMsg { facturas = facturasPorProcesar });
                }
                //if (config.Default.ptkEnviarPorFTP){
                //    var consumoFtpPtk = new ConsumoFtpPtkBusiness();
                //    consumoFtpPtk.LogNotificationEvent += (typelog, msg) => Log(typelog, msg);
                //    consumoFtpPtk.SendFacturasByFtpAsync(facturasPorProcesar);
                //}
                SetFacturasProcesadas(facturasPorProcesar);
            }
        }
        private void SetFacturasProcesadas(List<FacturaPromotickMsg> me)
        {
            facturasProcesadas = new List<FacturaPromotickMsg>();
            me.ForEach(factura => {
                facturasProcesadas.Add(factura);
            });
        }
        private bool SeProceso(List<FacturaPromotickMsg> facturasPorProcesar)
        {
            if (facturasProcesadas == null)
                return false;
            if (facturasProcesadas.Count != facturasPorProcesar.Count)
                return false;
            foreach (var factura in facturasPorProcesar) {
                //si al menos una de las facturas por procesar no esta en facturas procesadas
                if (!facturasProcesadas.Exists(f => f.numFactura == factura.numFactura))
                    return false;
            }
            return true; //cuando ya se proceso (facturasPorProcesar=facturasProcesadas)
        }
    }
}
