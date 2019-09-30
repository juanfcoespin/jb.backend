using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TechTools.Utils;
using jbp.msg;
using jbp.business.contracts;
using TechTools.DelegatesAndEnums;
using System.IO;

namespace jbp.business.promotick
{
    public class ConsumoFtpPtkBusiness : INotificationLog
    {
        public event dLogNotification LogNotificationEvent;

        #region Send Facturas y NC
        internal bool SendFacturasByFtpAsync(List<FacturaPromotickMsg> facturasPorProcesar)
        {
            var sourceFile = BuildExcelToSendByFtp(facturasPorProcesar);
            var credencials = GetFtpCredencials();

            var uploaded = FtpUtils.UploadFile(credencials, sourceFile);
            var msg = string.Format("Se subio el archivo por ftp: {0}, al sitio {1}",
                sourceFile, credencials.Url);
            if (uploaded)
                LogNotificationEvent?.Invoke(eTypeLog.Info, msg);
            else
                LogNotificationEvent?.Invoke(eTypeLog.Error, "No" + msg);
            return uploaded;
        }
        private static FtpUtils.Credencials GetFtpCredencials()
        {
            return new FtpUtils.Credencials()
            {
                Url = config.Default.ptkFtpUrl,
                User = config.Default.ptkFtpUser,
                Pwd = config.Default.ptkFtpPwd
            };
        }
        private string BuildExcelToSendByFtp(List<FacturaPromotickMsg> facturasPorProcesar)
        {
            var fileName = config.Default.ptkFtpFileName;
            if (config.Default.ptkFtpPutDateTimeOnFileName)
                fileName = FileUtils.AddTimeToFileName(fileName);
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
        #endregion
    }
}
