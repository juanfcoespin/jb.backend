using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using jbp.msg;
using jbp.business.oracle9i.contracts;
using jbp.core.oracle9i;
using TechTools.DelegatesAndEnums;
using TechTools.Rest;
using TechTools.Exceptions;

namespace jbp.business.oracle9i.promotick
{
    public class ConsumoWsPtkBusiness: INotificationLog
    {
        public event dLogNotification LogNotificationEvent;
        private RestCall.AuthenticationMe credencialesWsPromotick = null;

        public ConsumoWsPtkBusiness() {
            if (!string.IsNullOrEmpty(conf.Default.ptkWsUser))
                
                this.credencialesWsPromotick = new RestCall.AuthenticationMe
                {
                    User = conf.Default.ptkWsUser,
                    Pwd = conf.Default.ptkWsPwd,
                    AuthType = RestCall.eAuthType.Basic
                };
        }

        #region Envío Facturas y NC 
        public void SendFacturaToWsAsync(DocumentosPtkMsg me)
        {
            try
            {
                var url = string.Format("{0}/{1}", conf.Default.ptkWsUrl, "gsttransaccion");
                var rc = new RestCall();
                //rc.DataArrived += SendFacturaToWsAsync_Response;
                /*var resp=rc.SendPostOrPut(url, typeof(RespuestasPtkWsFacturasMsg),
                    me, typeof(FacturasPtkMsg), RestCall.eRestMethod.POST, this.credencialesWsPromotick);*/
                me.facturas.ForEach(factura =>{
                    LogNotificationEvent?.Invoke(eTypeLog.Info,
                    string.Format("Enviada Factura:{0}, ruc: {1}, monto:{2}, al servicio: {3}",
                        factura.numFactura, factura.numDocumento, factura.montoFactura, url));
                });
            }
            catch (Exception e)
            {
                e = ExceptionManager.GetDeepErrorMessage(e, ExceptionManager.eCapa.Business);
                LogNotificationEvent?.Invoke(eTypeLog.Error, e.Message);
            }
        }
        private void SendFacturaToWsAsync_Response(object resp, string err)
        {
            if (resp != null)
            {
                ((RespuestasPtkWsFacturasMsg)resp).respuesta.ForEach(item => {
                    if (item != null)
                        UpdateCodigoRespuestaFacturasWS(item);
                });
            }
            else
            {
                if (!string.IsNullOrEmpty(err))
                    LogNotificationEvent?.Invoke(eTypeLog.Error, err);
            }
        }
        private void UpdateCodigoRespuestaFacturasWS(RespPtkWSFacturasMsg resp)
        {
            try
            {
                new FacturaPromotickCore().UpdateCodigoRespWS(resp);
                var msg =
                    string.Format("Respuesta WS: numFactura: {0}, codigo:{1}, msg:{2}",
                    resp.numFactura, resp.codigo, resp.mensaje);
                try
                { //suele dar errores de concurrencia de los hilos
                    LogNotificationEvent?.Invoke(eTypeLog.Info, msg);
                }
                catch { }
            }
            catch (Exception e)
            {
                e = ExceptionManager.GetDeepErrorMessage(e, ExceptionManager.eCapa.Business);
                LogNotificationEvent?.Invoke(eTypeLog.Error, e.Message);
            }
        }
        #endregion

        #region Envio Aceleradores
        public void SendAceleradoresToWsAsync()
        {
            try
            {
                var me = new ParametroAceleradoresMsg() {
                    Año=conf.Default.ptkAceleradorAño,
                    Meses=conf.Default.ptkAceleradorMeses,
                    CodigosProductos=conf.Default.ptkAceleradorCodProductos
                };
                LogNotificationEvent?.Invoke(eTypeLog.Info,
                    string.Format("Iniciando el envio de aceleradores del año: {0}, meses: {1}, productos: {2}", me.Año, me.Meses, me.CodigosProductos));
                var url = string.Format("{0}/{1}", conf.Default.ptkWsUrl, "gstacelerador");
                var facturaPromotickCore = new FacturaPromotickCore();
                var listAceleradores = facturaPromotickCore.GetListAceleradores(me);
                if (listAceleradores != null && listAceleradores.Count > 0)
                {
                    var rc = new RestCall();
                    listAceleradores.ForEach(acelerador => {
                        facturaPromotickCore.InsertarAceleradorEnviado(acelerador);
                        LogNotificationEvent?.Invoke(eTypeLog.Info, 
                            string.Format("Enviado Nro Documento: {0}, Puntos: {1}",acelerador.NroDocumento,acelerador.puntos));
                        var resp = (RespPtkAcelerador)rc.SendPostOrPut(url, typeof(RespPtkAcelerador), acelerador, 
                            typeof(AceleradorMsg), RestCall.eRestMethod.POST, this.credencialesWsPromotick);
                        if (resp != null) {
                            facturaPromotickCore.UpdateRespAceleradorWS(resp);
                            LogNotificationEvent?.Invoke(eTypeLog.Info,
                                string.Format("Respuesta WS: Cod:{0}, msg: {1}",resp.codigo, resp.mensaje));
                        }
                    });
                }
            }
            catch (Exception e)
            {
                e = ExceptionManager.GetDeepErrorMessage(e, ExceptionManager.eCapa.Business);
                LogNotificationEvent?.Invoke(eTypeLog.Error, e.Message);
            }
        }
        #endregion
    }
}
