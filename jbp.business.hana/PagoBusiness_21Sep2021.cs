using jbp.msg.sap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jbp.core.sapDiApi;
using TechTools.Core.Hana;
using System.Threading;
using System.Data;



namespace jbp.business.hana
{
    public class PagoBusiness_21Sep2021: BaseBusiness
    {
        //para controlar la concurrencia
        public static readonly object control = new object();
        public static SapPagoRecibido sapPagoRecibido = new SapPagoRecibido();

        public List<string> SavePagos(List<PagoMsg> pagos)
        {
            /*
            var ms = new List<string>();
            pagos.ForEach(pago =>
            {
                ms.Add("Temporalmente supendido el registro de pagos, por correcciones en cartera");
            });
            return ms;
            */
            
            Monitor.Enter(control);
            try
            {
                var ms = ProcessPagos(pagos);
                return ms;
            }
            finally
            {
                Monitor.Exit(control);
            }
        }
        private List<string> ProcessPagos(List<PagoMsg> pagos)
        {
            var ms = new List<string>();
            if (pagos != null && pagos.Count > 0)
            {
                if (sapPagoRecibido == null)
                    sapPagoRecibido = new SapPagoRecibido();
                if (!sapPagoRecibido.IsConected())
                {
                    var seConecto=sapPagoRecibido.Connect();//se conecta a sap
                }
                pagos.ForEach(pago =>
                {
                    try
                    {
                        var resp = "";
                        if (DuplicatePago(pago))
                            resp = "Anteriormente ya se procesó este item!";
                        else
                        {
                            pago.facturasAPagar.ForEach(factura => {
                                var folioNum = GetNumFolio(factura.numDoc);
                                factura.DocEntry = FacturaBusiness.GetDocEntryFromFolioNum(folioNum);
                            });
                            resp = sapPagoRecibido.Add(pago);
                            if (resp == "ok")
                            {
                                EnviarCorreoPago(pago);
                            }
                        }
                        ms.Add(resp);
                    }
                    catch (Exception e)
                    {
                        ms.Add(e.Message);
                    }
                });
            }
            return ms;
        }
        
        private void EnviarCorreoPago(PagoMsg pago)
        {
            string titulo = "Pago Recibido - " + pago.client;
            string msg = string.Empty;
            var bddName = BaseCore.GetBddName();
            msg += string.Format(@"
                <h2>{5}</h2><br>
                <b>Cliente:</b> {0} <br>
                <b>CodCliente:</b> {1} <br>
                <b>Monto Pagado:</b> USD {2} <br>
                <b>Base de datos:</b> {3} <br>
                <b>Comentario:</b><br>{4} <br><br>
            ", pago.client, pago.CodCliente, pago.totalPagado,bddName , pago.comment, titulo);
            msg += "<b>Facturas Pagadas:</b> <br>";
            msg += "<table>";
            msg += " <tr>";
            msg += "    <td style='border: solid 1px #000000'><b>Num Factura</b></td>";
            msg += "    <td style='border: solid 1px #000000'><b>Total Factura</b></td>";
            msg += "    <td style='border: solid 1px #000000'><b>Saldo Vencido</b></td>";
            msg += "    <td style='border: solid 1px #000000'><b>Pagado</b></td>";
            msg += "    <td style='border: solid 1px #000000'><b>Saldo Pendiente</b></td>";
            msg += " </tr>";
            pago.facturasAPagar.ForEach(factura => {
                var saldoPendiente = factura.toPay - factura.pagado;
                msg += "<tr>";
                msg += "    <td style='border: solid 1px #000000'>" + factura.numDoc+"</td>";
                msg += "    <td style='border: solid 1px #000000'>USD " + factura.total + "</td>";
                msg += "    <td style='border: solid 1px #000000'>USD " + factura.toPay+"</td>";
                msg += "    <td style='border: solid 1px #000000'>USD " + factura.pagado + "</td>";
                msg += "    <td style='border: solid 1px #000000'>USD " + saldoPendiente.ToString("#.##") + "</td>";
                msg += "</tr>";
            });
            msg += "</table><br>";
            msg += "<b>Detalles del Pago:</b> <br>";
            msg += "<table>";
            msg += " <tr>";
            msg += "    <td style='border: solid 1px #000000'><b>Tipo Pago</b></td>";
            msg += "    <td style='border: solid 1px #000000'><b>Monto</b></td>";
            msg += "    <td style='border: solid 1px #000000'><b>Banco</b></td>";
            msg += " </tr>";
            pago.tiposPago.ForEach(tp => {
                msg += "<tr>";
                msg += "    <td style='border: solid 1px #000000'>" + tp.tipoPago + "</td>";
                msg += "    <td style='border: solid 1px #000000'>USD " + tp.monto + "</td>";
                msg += "    <td style='border: solid 1px #000000'>" + tp.bancoTxt + "</td>";
                msg += "</tr>";
            });
            msg += "</table><br>";
            if (!string.IsNullOrEmpty(pago.photoComprobanteData))
            {
                msg += "<b>Comprobante Pago:</b> <br>";
                msg += string.Format(@"<img src=""data: image / png; base64, {0}""/>", pago.photoComprobanteData);
            }
            msg += "<div><i><b>Nota: </b>Los pagos detallados en este correo están sujetos a revisión del departamento de cobranzas de James Brown Pharma</div></i>";
            var destinatarios = conf.Default.correoPagos;
            //para que no se envíe al cliente en ambiente de pruebas
            if (!bddName.ToLower().Contains("prueba"))
            {
                var correoCliente = SocioNegocioBusiness.GetCorreoByCodigo(pago.CodCliente);
                if (correoCliente != null && TechTools.Utils.ValidacionUtils.EmailValid(correoCliente))
                    destinatarios += "; " + correoCliente;
            }
            
            this.EnviarPorCorreo(destinatarios, titulo, msg);
        }

        private static bool DuplicatePago(PagoMsg pago)
        {
            return false;
        }
        private static int GetNumFolio(string numDoc)
        {
            //001-010-000081691
            var matriz = numDoc.Split(new char[] { '-' });
            var ex = new Exception("Numero de documento de factura incorrecto: " + numDoc);
            if (matriz != null && matriz.Length > 0)
            {
                var folioStr = matriz[matriz.Length - 1];
                try
                {
                    return Convert.ToInt32(folioStr); //se quitan los 0s de la izq
                }
                catch
                {
                    throw ex;
                }
            }
            else
                throw ex;
        }
    }
}
