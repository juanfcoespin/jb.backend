using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using jbp.business.hana;
using jbp.msg;
using TechTools.Net;

namespace jb.presentacion.reporteDiarioCobros
{

    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var vendedores = VendedorBusiness.GetVendedoresConCobros();
                EnviarReporte(vendedores);
            }
            catch (Exception e)
            {
                NotificarError(e.Message);
            }
        }

        private static void EnviarReporte(List<VendedorConCobrosMsg> vendedores)
        {
            
            vendedores.ForEach(vendedor => {
                if (vendedor.Cobros.Count > 0) {
                    string reporte = getReporte(vendedor);
                    string error = null;
                    string destinatarios = vendedor.Email;
                    destinatarios += ";" + Properties.conf.Default.destinatarios;
                    MailUtils.SendAndGetError(ref error, destinatarios, "Reporte Cobros Diarios - "+vendedor.Nombre, reporte);
                    if (error != null)
                        NotificarError(error, true);
                }
            });
        }

        

        private static string getReporte(VendedorConCobrosMsg vendedor)
        {
            var tableStyle = "style=\"width: 100%; border-collapse: collapse; border: 1px solid #ddd;\"";
            var thStyle = "style=\"background-color: #454443; color:#FFF; text-align: left; padding: 8px; border: 1px solid #ddd;\"";
            var thStyle2 = "style=\"background-color: #f4f4f4; text-align: left; padding: 8px; border: 1px solid #ddd;\"";
            var tdStyle = "style=\"padding: 8px; border: 1px solid #ddd;\"";

            var ms = string.Format(@"
                <div>Estimad@ <b>{0}</b></div>
                <div>A continuación su reporte de cobros diarios:</div><br>
                <div><b>Total Cobrado: {1} USD</b></div><br>
            ", vendedor.Nombre, getTotalCobradoVendedor(vendedor));
           
            
            vendedor.Cobros.ForEach(cobro =>
            {
                ms += "<table " + tableStyle + ">";
                ms += string.Format(@"
                    <thead>
                        <th {0}>Fecha</th>
                        <th {0}>Estado</th>
                        <th {0}>Cliente</th>
                        <th {0}>Total</th>
                    </thead>
                ", thStyle);
                ms += "<tbody>";
                ms += string.Format(@"
                <tr>
                    <td {3}>{0}</td>
                    <td {3}>{1}</td>
                    <td {3}>{2}</td>
                    <td {3}>{3} USD</td>
                </tr>
                ", cobro.Fecha, cobro.Estado, cobro.Cliente, cobro.TotalCobrado, tdStyle);
                
                var ms1 = "<table " + tableStyle + ">";
                
                ms1 += string.Format(@"
                    <thead>
                        <th {0}>Tipo Pago</th>
                        <th {0}>Numero Tranferencia</th>
                        <th {0}>Fecha</th>
                        <th {0}>CtaBancoJB</th>
                        <th {0}>MontoCheque</th>
                        <th {0}>FechaVencimiento</th>
                        <th {0}>NroCheque</th>
                        <th {0}>Banco</th>
                    </thead>
                ", thStyle2);
                cobro.Pagos.ForEach(pago => {
                    ms1 += string.Format(@"
                        <tr>
                            <td {0}>{1}</td>
                            <td {0}>{2}</td>
                            <td {0}>{3}</td>
                            <td {0}>{4}</td>
                            <td {0}>{5}</td>
                            <td {0}>{6}</td>
                            <td {0}>{7}</td>
                            <td {0}>{8}</td>
                        </tr>
                    ", tdStyle, pago.TipoPago, pago.NumTransferencia, pago.FechaTransferencia, pago.CtaBancoJB,
                    pago.MontoCheque, pago.FechaVencimientoCheque,pago.NroCheque, pago.BancoCheque);
                });
                ms1 += "</table >";
                ms += string.Format(@"
                <tr>
                    <td colspan=""3"" style=""padding-left:20px"">
                        <div><b>Detalle de pagos:</b></div>
                       {0}
                    </td>
                </tr>
                ", ms1);
                ms += "</tbody>";
                ms += "</table>";
            });
            
            return ms;
        }

        private static string getTotalCobradoVendedor(VendedorConCobrosMsg vendedor)
        {
            decimal ms = 0;
            vendedor.Cobros.ForEach(cobro => ms += cobro.TotalCobrado);
            return ms.ToString();
        }

        private static void NotificarError(string message, bool soloLog=false)
        {
            try
            {
                if (!soloLog)
                    MailUtils.Send(Properties.conf.Default.correoErrores, "Error al enviar el reporte diario de cobranza", message);
            }
            catch{}
        }
    }
}
