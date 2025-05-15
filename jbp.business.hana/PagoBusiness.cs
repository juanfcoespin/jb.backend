using jbp.msg.sap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jbp.core.sapDiApi;
using TechTools.Core.Hana;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using System.ComponentModel;
using TechTools.Net;


namespace jbp.business.hana
{
    public class PagoBusiness: BaseBusiness
    {
        //para controlar la concurrencia
        public static readonly object control = new object();
        public static SapPagoRecibido sapPagoRecibido = new SapPagoRecibido();

        public List<string> SavePagos(List<PagosMsg> pagos)
        {
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
        private List<string> ProcessPagos(List<PagosMsg> pagos)
        {
            var ms = new List<string>();
            var correosPendientes=new List<PagosMsg>();
            
            if (pagos != null && pagos.Count > 0)
            {
                var clientId = pagos[0].ClientId;
                pagos.ForEach(pago =>
                {
                    var numIntentos = 1;
                    try
                    {
                        SendMessageToClient(clientId, "Procesando pago(s) del cliente " + pago.client, eMessageType.Success);
                        ProcessPago(pago, correosPendientes, numIntentos, clientId);
                        SendMessageToClient(clientId, "Pago Registrado Correctamente!!", eMessageType.Success);
                        ms.Add("ok");
                    }
                    catch (Exception e)
                    {
                        var error = "Error al procesar el pago";
                        error+=e.Message;
                        error += e.StackTrace;
                        ms.Add(error);
                        SendMessageToClient(clientId, "Error al procesar el pago: " + error, eMessageType.Error);
                    }
                });
                SendMessageToClient(clientId, "Enviando correos electrónicos");
                // se envían los correos en otro hilo
                EnviarCorreosAsync(correosPendientes);
            }
            return ms;
        }


        private static void ProcessPago(PagosMsg pago, List<PagosMsg> correosPendientes, int numIntentos, string clientId=null)
        {
            if (numIntentos > 3)
                throw new Exception("Se ha tratado de procesar este pago por 3 veces y no se ha podido establecer conexión con SAP!!");
            var pagoBk = (PagosMsg)pago.Clone();
            
            SendMessageToClient(clientId, "Conectando a SAP ...");
            ConectarASap();
            SendMessageToClient(clientId, "Conectado a SAP correctamente");
            try
            {
                SendMessageToClient(clientId, "Validando si no hay pagos duplicados ...");
                if (DuplicatePago(pago))
                {
                    SendMessageToClient(clientId, "Pago duplicado identificado");
                    throw new Exception("Anteriormente ya se procesó este item!");
                }
                else
                {
                    pago.facturasAPagar.ForEach(factura =>
                    {
                        SendMessageToClient(clientId, "Registrando pago factura "+factura.numDoc,eMessageType.Warning);
                        var folioNum = GetNumFolio(factura.numDoc);
                        factura.folioNum = folioNum;
                        if (factura.IdFactura == 0)
                            throw new Exception("Debe actualizar el aplicativo a la versión 3.4.7.0 o superior");
                        factura.DatosAdicionales = FacturaBusiness.GetDatosFactura(factura.IdFactura);
                        if (factura.DatosAdicionales != null && factura.tipoDocumento != "Cheque Protestado")
                            factura.DocEntry = factura.DatosAdicionales.Id;
                        if (factura.tipoDocumento == "Cheque Protestado")
                            factura.DocEntry = FacturaBusiness.GetIdChequeProtestadoByDocNum(factura.numDoc);
                    });
                    sapPagoRecibido.OnMessageArrived += (string msg) =>
                    {
                        SendMessageToClient(clientId, msg);
                    };
                    sapPagoRecibido.SafePago(pago);
                    correosPendientes.Add(pago);
                    
                }
            }
            catch (Exception e)
            {
                if (e.Message == "You are not connected to a company" || e.Message.Contains("RPC_E_SERVERFAULT"))
                {
                    //me vuelvo a conectar y reproceso
                    sapPagoRecibido = null;
                    numIntentos++;
                    ProcessPago(pagoBk, correosPendientes, numIntentos);
                }else
                    throw e;
            }
        }

        private static void ConectarASap()
        {
            if (sapPagoRecibido == null)
                sapPagoRecibido = new SapPagoRecibido();

            if (!sapPagoRecibido.IsConected())
            {
                if (!sapPagoRecibido.Connect()) // cuando no se puede conectar es por que el obj sap se inhibe
                {
                    sapPagoRecibido = null;
                    sapPagoRecibido = new SapPagoRecibido(); //se reinicia el objeto para hacer otro intento de conexión
                    if (!sapPagoRecibido.Connect())
                    {
                        sapPagoRecibido = null;
                        throw new Exception("Alta concurrencia: Vuelva a intentar la sincronización en 1 minuto");
                    }
                }
            }
        }

        private static void EnviarCorreosAsync(List<PagosMsg> pagos) {
            if (pagos == null || pagos.Count == 0)
                return;
            var ec = new EnvioCorreoPago();
            ec.pagos = pagos;
            Task.Run(() => ec.Enviar());
        }

        private static bool DuplicatePago(PagosMsg pago)
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

    public class EnvioCorreoPago : BaseBusiness
    {
        public List<PagosMsg> pagos { get; set; }
        public EnvioCorreoPago()
        {
            this.pagos = new List<PagosMsg>();
        }
        public void Enviar()
        {
            this.pagos.ForEach(pago =>
            {
                this.EnviarCorreoPago(pago);
            });
        }
        private string downloadComprobatePago(string photoComprobanteData, string nombreCoprobantePago)
        {
            byte[] imageBytes = Convert.FromBase64String(photoComprobanteData);
            var pathComprobantesPago = conf.Default.pathComprobantesPago;
            if (!Directory.Exists(pathComprobantesPago))
                Directory.CreateDirectory(pathComprobantesPago);
            var compleateImgPath = string.Format(@"{0}\{1}", conf.Default.pathComprobantesPago,
                nombreCoprobantePago);
            TechTools.Utils.FileUtils.CreateFile_FromByteArray(compleateImgPath, imageBytes);
            return compleateImgPath;
        }
        private void EnviarCorreoPago(PagosMsg pago)
        {
            var vendedorObj = SocioNegocioBusiness.GetVendedorByCodSocioNegocio(pago.CodCliente);
            var vendedor = (vendedorObj != null) ? vendedorObj.Vendedor : null;
            var correoVendedor = (vendedorObj != null) ? vendedorObj.Correo : null;

            string titulo = string.Format("Pago Recibido - ({0}): {1}", vendedor, pago.client);
            string msg = string.Empty;
            var bddName = BaseCore.GetBddName();
            var fotosPath = new List<string>();
            var i = 0;
            if (pago.fotosComprobantes != null)
            {
                pago.fotosComprobantes.ForEach(foto => {
                    i++;
                    var nombreCoprobantePago = getNombreComprobantePago(pago.CodCliente, i);
                    fotosPath.Add(downloadComprobatePago(foto, nombreCoprobantePago));
                });
            }
            var totalPagado = pago.GetTotalPagado();
            msg += string.Format(@"
                <h2>Recibo de Cobro: {3}</h2>
                <h2>Total Pagado: {4}</h2><br>
                <b>Vendedor:</b> {0} <br>                
                <b>Cliente:</b> {1} <br>
                <b>CodCliente:</b> {2} <br>
                <b>Base de datos:</b> {5} <br>
                <b>Comentario:</b><br>{6} <br><br>
            ", vendedor, pago.client, pago.CodCliente, pago.numRecibo, totalPagado, bddName, pago.comment, titulo);

            var hayDescuentoPP = false;
            foreach (var factura in pago.facturasAPagar) {
                if (factura.descuentoPP > 0)
                {
                    hayDescuentoPP = true;
                    break;
                }
            }
            msg += "<h3><u>Facturas Pagadas</u>:</h3>";
            msg += "<table>";
            msg += " <tr>";
            msg += "    <td style='border: solid 1px #000000'><b>Num Factura</b></td>";
            msg += "    <td style='border: solid 1px #000000'><b>Fec Factura</b></td>";
            msg += "    <td style='border: solid 1px #000000'><b>FecVen Factura</b></td>";
            msg += "    <td style='border: solid 1px #000000'><b>Total Factura</b></td>";
            msg += "    <td style='border: solid 1px #000000'><b>Saldo Factura</b></td>";
            msg += "    <td style='border: solid 1px #000000'><b>Valor Pagado</b></td>";
            if (hayDescuentoPP) {
                msg += "    <td style='border: solid 1px #000000'><b>% Desc. Pronto Pago</b></td>";
                msg += "    <td style='border: solid 1px #000000'><b>Descuento PP</b></td>";
                
            }
            msg += " </tr>";
            //TODO: Incluir en el correo el valor de la retención aplicada
            pago.facturasAPagar.ForEach(factura => {
                msg += "<tr>";
                msg += "    <td style='border: solid 1px #000000'>" + factura.numDoc + "</td>";
                msg += "    <td style='border: solid 1px #000000'>" + factura.date + "</td>";
                msg += "    <td style='border: solid 1px #000000'>" + factura.dueDate + "</td>";
                msg += "    <td style='border: solid 1px #000000'>USD " + factura.total + "</td>";
                msg += "    <td style='border: solid 1px #000000'>USD " + factura.toPay + "</td>";
                msg += "    <td style='border: solid 1px #000000'>USD " + factura.valorPagado + "</td>";
                if (hayDescuentoPP) {
                    msg += "    <td style='border: solid 1px #000000'>" + factura.porcentajePP + "%</td>";
                    msg += "    <td style='border: solid 1px #000000'>USD " + Math.Round(factura.descuentoPP, 2) + "</td>";
                    
                }
                if (!string.IsNullOrEmpty(factura.comentarioCobroPorExcepcion))
                    msg += "    <td style='border: solid 1px #000000'><b>Comentario cobro por excepción: </b>" + factura.comentarioCobroPorExcepcion + "</td>";
                msg += "</tr>";
            });
            msg += "</table><br>";

            msg += "<br><h3><u>Detalle de Pagos</u>:</h3>";
            int j = 0;
            pago.tiposPagoToSave.ForEach(tp => {
                if(j>0)
                    msg += "<hr>";
                msg += "<div>";
                msg += "<p><b>Tipo Pago:&nbsp;</b>" + tp.tipoPago + "</p>";
                if (tp.tipoPago=="Efectivo" || tp.tipoPago == "Transferencia")
                    msg += "<p><b>Monto:&nbsp;</b>" + tp.monto + "USD</p>";
                if (tp.tipoPago == "Transferencia") {
                    msg += "<p><b>Fecha Transferencia:&nbsp;</b>" + tp.fechaTransferencia + "</p>";
                    msg += "<p><b>Banco:&nbsp;</b>" + tp.bancoTxt + "</p>";
                    msg += "<p><b>Num Transfer:&nbsp;</b>" + tp.NumTransferencia + "</p>";
                }
                if (tp.tipoPago.ToLower().Contains("cheque"))
                {
                    tp.cheques.ForEach(cheque => {
                        msg += "<p><b>Monto Cheque:&nbsp;</b>" + cheque.monto + "USD</p>";
                        msg += "<p><b>Banco:&nbsp;</b>" + cheque.bancoTxt + "</p>";
                        msg += "<p><b>Num Cheque:&nbsp;</b>" + cheque.NumCheque + "</p>";
                        if (cheque.FechaVencimientoCheque != null)
                            msg += "<p><b>Fecha Vencimiento:&nbsp;</b>" + cheque.FechaVencimientoCheque.ToString("yyyy-MM-dd") + "</p>";
                    });
                }
                msg += "</div>";
                j++;
            });

            
            msg += "<br>";
            msg += "<hr>";
            msg += "<div><i><b>Nota: </b>Los pagos detallados en este correo están sujetos a revisión del departamento de cobranzas de James Brown Pharma</div></i>";

            var destinatarios = string.Format("{0}; {1}", conf.Default.correoPagos, correoVendedor);
            //para que no se envíe al cliente en ambiente de pruebas
            if (bddName.Equals("SBO_JBP_PROD")) //si es la base de datos de produccion
            {
                var correoCliente = SocioNegocioBusiness.GetCorreoByCodigo(pago.CodCliente);
                if (correoCliente != null && TechTools.Utils.ValidacionUtils.EmailValid(correoCliente))
                    destinatarios += "; " + correoCliente;
            }
            this.EnviarPorCorreo(destinatarios, titulo, msg, fotosPath);
        }
        private string getNombreComprobantePago(string codCliente, int numFile)
        {
            var ms = string.Format("{0}_{1}_{2}.png", codCliente, DateTime.Now.ToString("yyyyMMddhhmm"), numFile.ToString());
            return ms;
        }
    }
}
