using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using jbp.msg;
using jbp.core;
using TechTools.Utils;
using TechTools.DelegatesAndEnums;
using System.IO;
using jbp.business.contracts;

namespace jbp.business
{
    public class FacturaPromotickBusiness:INotificationLog
    {
        public event dLogNotification LogNotificationEvent;
        internal static bool EsFacturaPromotic(int idFactura)
        {
            return new FacturaPromotickCore().EsFacturaPromotic(idFactura);
        }
        internal static FacturaPromotickMsg GetById(int idFactura)
        {
            try
            {
                return new FacturaPromotickCore().GetById(idFactura);
            }
            catch (Exception e)
            {
                e = ExceptionManager.GetDeepErrorMessage(e, ExceptionManager.eCapa.Business);
                return new FacturaPromotickMsg { Error = e.Message };
            }
        }
        internal List<FacturaPromotickMsg> GetCurrentMonthFacturasToSendWS()
        {
            var ms = new List<FacturaPromotickMsg>();
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
        internal bool SendFacturasByFtpAsync(List<FacturaPromotickMsg> facturasPorProcesar)
        {
            var sourceFile = BuildExcelToSendByFtp(facturasPorProcesar);
            var credencials = GetFtpCredencials();
            
            var uploaded= FtpUtils.UploadFile(credencials, sourceFile);
            var msg = string.Format("Se subio el archivo por ftp: {0}, al sitio {1}",
                sourceFile,credencials.Url);
            if (uploaded)
                LogNotificationEvent?.Invoke(eTypeLog.Info, msg);
            else
                LogNotificationEvent?.Invoke(eTypeLog.Error, "No" +msg);
            return uploaded;
        }
        private static FtpUtils.Credencials GetFtpCredencials()
        {
            return new FtpUtils.Credencials()
            {
                Url = config.Default.ftpPromotickUrl,
                User = config.Default.ftpPromotickUser,
                Pwd = config.Default.ftpPromotickPwd
            };
        }
        private string BuildExcelToSendByFtp(List<FacturaPromotickMsg> facturasPorProcesar)
        {
            var fileName = config.Default.ftpPromotickFileName;
            if(config.Default.ftpPromotickPutDateTimeOnFileName)
                fileName= FileUtils.AddTimeToFileName(fileName);
            var ex = new ExcelUtils(fileName);
            ex.AddColumn("Fecha Factura");
            ex.AddColumn("Numero Factura");
            ex.AddColumn("Descripcion");
            ex.AddColumn("Numero Documento");
            ex.AddColumn("Monto");
            ex.AddColumn("Puntos");
            var row = 1;// empieza en 1 porque la primera fila es para los títulos
            facturasPorProcesar.ForEach(factura => {
                ex.AddData(row, 0, factura.fechaFactura);
                ex.AddData(row, 1, factura.numFactura);
                ex.AddData(row, 2, factura.descripcion);
                ex.AddData(row, 3, factura.numDocumento);
                ex.AddData(row, 4, factura.montoFactura);
                ex.AddData(row, 5, factura.puntos);
                row++;
            });
            if (FileUtils.HasFolders(fileName) && !Directory.Exists(fileName))
                FileUtils.CreateFileDirectory(fileName);
            ex.EndEditAndSave();
            return fileName;
        }
        internal void SendFacturaToWsAsync(List<FacturaPromotickMsg> me)
        {
            try
            {
                var url = config.Default.urlWSPromotick;
                var restCall = new RestCall();
                restCall.DataArrived += (result, errorMsg) => {
                    if (result != null)
                    {
                        var listResp = (List<ParametroSalidaPtkMsg>)result;
                        listResp.ForEach(resp => {
                            if (resp != null)
                                UpdateCodigoRespuestaWS(resp);
                        });
                    }
                };
                restCall.SendPostOrPutAsync(url, typeof(List<ParametroSalidaPtkMsg>),
                    me, typeof(List<FacturaPromotickMsg>), RestCall.eRestMethod.POST);
                me.ForEach(factura => {
                    LogNotificationEvent?.Invoke(eTypeLog.Info,
                    String.Format("Enviada Factura:{0}, ruc: {1}, monto:{2}, al servicio: {3}",
                        factura.numFactura, factura.numDocumento, factura.montoFactura, url));
                });
            }
            catch (Exception e)
            {
                e = ExceptionManager.GetDeepErrorMessage(e, ExceptionManager.eCapa.Business);
                LogNotificationEvent?.Invoke(eTypeLog.Error, e.Message);
            }
        }
        private void UpdateCodigoRespuestaWS(ParametroSalidaPtkMsg resp)
        {
            try
            {
                new FacturaPromotickCore().UpdateCodigoRespWS(resp);
                var msg= 
                    string.Format("Respuesta WS: numFactura: {0}, codigo:{1}, msg:{2}",
                    resp.numFactura, resp.codigo, resp.mensaje);
                try{ //suele dar errores de concurrencia de los hilos
                    LogNotificationEvent?.Invoke(eTypeLog.Info, msg);
                }
                catch {}
                
                    
            }
            catch (Exception e)
            {
                e = ExceptionManager.GetDeepErrorMessage(e, ExceptionManager.eCapa.Business);
                LogNotificationEvent?.Invoke(eTypeLog.Error, e.Message);
            }
        }
        public void InsertFacturasEnviadasAPromotick(List<FacturaPromotickMsg> me) {
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
