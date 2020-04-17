using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using jbp.msg;
using jbp.core.oracle9i;
using TechTools.DelegatesAndEnums;
using System.IO;
using jbp.business.oracle9i.contracts;
using TechTools.Exceptions;
using TechTools.Utils;

namespace jbp.business.oracle9i.promotick
{
    public class FacturaPromotickBusiness:INotificationLog
    {
        public event dLogNotification LogNotificationEvent;

        public FacturaPromotickBusiness() {
        }
        
        internal static bool EsFacturaPromotic(int idFactura)
        {
            return new FacturaPromotickCore().EsFacturaPromotic(idFactura);
        }
        internal static DocumentoPromotickMsg GetById(int idFactura)
        {
            try
            {
                return new FacturaPromotickCore().GetById(idFactura);
            }
            catch (Exception e)
            {
                e = ExceptionManager.GetDeepErrorMessage(e, ExceptionManager.eCapa.Business);
                return new DocumentoPromotickMsg { Error = e.Message };
            }
        }
        internal List<DocumentoPromotickMsg> GetCurrentMonthFacturasToSendWS()
        {
            var ms = new List<DocumentoPromotickMsg>();
            try
            {
                var currentMonth = string.Format("{0}-{1}", DateTime.Now.Year,
                    StringUtils.getTwoDigitNumber(DateTime.Now.Month));
                ms= new FacturaPromotickCore().GetFacturasToSendWsByMonth(currentMonth);                
            }
            catch (Exception e)
            {
                e = ExceptionManager.GetDeepErrorMessage(e, ExceptionManager.eCapa.Business);
                this.LogNotificationEvent?.Invoke(eTypeLog.Error, e.Message);
            }
            return ms;

        }
        internal void InsertFacturasEnviadasAPromotick(List<DocumentoPromotickMsg> me)
        {
            me.ForEach(factura =>
            {
                try
                {
                    new FacturaPromotickCore().InsertFactEnviada(factura);
                    LogNotificationEvent?.Invoke(eTypeLog.Info,
                        string.Format("Enviada Factura: {0}, cliente: {1}, monto: {2} ",
                            factura.numFactura, factura.numDocumento, factura.montoFactura));
                }
                catch (Exception e)
                {
                    e = ExceptionManager.GetDeepErrorMessage(e, ExceptionManager.eCapa.Business);
                    this.LogNotificationEvent?.Invoke(eTypeLog.Error, e.Message);
                }
            });
        }
    }
}
