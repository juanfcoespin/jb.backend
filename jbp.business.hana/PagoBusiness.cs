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
using System.IO;



namespace jbp.business.hana
{
    public class PagoBusiness: BaseBusiness
    {
        //para controlar la concurrencia
        public static readonly object control = new object();
        public static SapPagoRecibido sapPagoRecibido = new SapPagoRecibido();

        public List<string> SavePagos(List<PagosMsg> pagos)
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
        private List<string> ProcessPagos(List<PagosMsg> pagos)
        {
            var ms = new List<string>();
            if (pagos != null && pagos.Count > 0)
            {
                if (sapPagoRecibido == null)
                    sapPagoRecibido = new SapPagoRecibido();
                
                if (!sapPagoRecibido.IsConected())
                {
                    if (!sapPagoRecibido.Connect()) // cuando no se puede conectar es por que el obj sap se inhibe
                    {
                        // en el próximo intento obliga a crear otra vez el objeto para hacer una conexión limpia
                        sapPagoRecibido = null; 
                        throw new Exception("Alta concurrencia: Vuelva a intentar la sincronización en 1 minuto");
                    }
                }
                pagos.ForEach(pago =>{
                    try{
                        var resp = "";
                        if (DuplicatePago(pago))
                            resp = "Anteriormente ya se procesó este item!";
                        else{
                            pago.facturasAPagar.ForEach(factura => {
                                var folioNum = GetNumFolio(factura.numDoc);
                                factura.folioNum = folioNum;
                                factura.DatosAdicionales = FacturaBusiness.GetDatosFactura(folioNum);
                                if(factura.DatosAdicionales!=null && factura.tipoDocumento != "Cheque Protestado")
                                    factura.DocEntry = factura.DatosAdicionales.Id;
                                if (factura.tipoDocumento == "Cheque Protestado")
                                    factura.DocEntry = FacturaBusiness.GetIdChequeProtestadoByDocNum(factura.numDoc);
                            });
                            resp = sapPagoRecibido.SafePagos(pago);
                            if (resp == "ok"){
                                EnviarCorreoPago(pago);
                            }
                        }
                        ms.Add(resp);
                    }
                    catch (Exception e){
                        ms.Add(e.Message);
                    }
                });
            }
            return ms;
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
                    var nombreCoprobantePago = getNombreComprobantePago(pago.CodCliente,i);
                    fotosPath.Add(downloadComprobatePago(foto, nombreCoprobantePago));
                });
            }
            var totalPagado = pago.GetTotalPagado();
            msg += string.Format(@"
                <h2>{5}</h2><br>
                <b>Vendedor:</b> {0} <br>                
                <b>Cliente:</b> {1} <br>
                <b>CodCliente:</b> {2} <br>
                <b>Nro Recibo:</b> {3} <br>
                <b>Monto Pagado:</b> USD {4} <br>
                <b>Base de datos:</b> {5} <br>
                <b>Comentario:</b><br>{6} <br><br>
            ",vendedor, pago.client, pago.CodCliente, pago.numRecibo, totalPagado, bddName , pago.comment, titulo);
            
            msg += "<b>Facturas Pagadas:</b> <br>";
            msg += "<table>";
            msg += " <tr>";
            msg += "    <td style='border: solid 1px #000000'><b>Num Factura</b></td>";
            msg += "    <td style='border: solid 1px #000000'><b>Fec Factura</b></td>";
            msg += "    <td style='border: solid 1px #000000'><b>FecVen Factura</b></td>";
            msg += "    <td style='border: solid 1px #000000'><b>Total Factura</b></td>";
            msg += "    <td style='border: solid 1px #000000'><b>Saldo Factura</b></td>";
            msg += "    <td style='border: solid 1px #000000'><b>% Desc. Pronto Pago</b></td>";
            msg += "    <td style='border: solid 1px #000000'><b>Descuento PP</b></td>";
            msg += "    <td style='border: solid 1px #000000' colspan='2'><b>Valor a Pagar</b></td>";
            msg += " </tr>";
            //TODO: Incluir en el correo el valor de la retención aplicada
            pago.facturasAPagar.ForEach(factura => {
                msg += "<tr>";
                msg += "    <td style='border: solid 1px #000000'>" + factura.numDoc+"</td>";
                msg += "    <td style='border: solid 1px #000000'>" + factura.date + "</td>";
                msg += "    <td style='border: solid 1px #000000'>" + factura.dueDate + "</td>";
                msg += "    <td style='border: solid 1px #000000'>USD " + factura.total + "</td>";
                msg += "    <td style='border: solid 1px #000000'>USD " + factura.toPay +"</td>";
                msg += "    <td style='border: solid 1px #000000'>" + factura.porcentajePP + "%</td>";
                msg += "    <td style='border: solid 1px #000000'>USD " + Math.Round(factura.descuentoPP,2) + "</td>";
                msg += "    <td style='border: solid 1px #000000'>USD " + factura.toPayMasProntoPago + "</td>";
                if(!string.IsNullOrEmpty(factura.comentarioCobroPorExcepcion))
                    msg += "    <td style='border: solid 1px #000000'><b>Comentario cobro por excepción: </b>" + factura.comentarioCobroPorExcepcion + "</td>";
                msg += "</tr>";
            });
            msg += "</table><br>";
            msg += "<b>Detalles del Pago:</b> <br>";
            msg += "<b>Total Pagado: USD " + totalPagado + "</b><br>";
            msg += @"<table>
                        <thead>
                            <th style='border: solid 1px #000000'><b>Tipo Pago</b></th>
                            <th style='border: solid 1px #000000'><b>Monto</b></th>
                            <th style='border: solid 1px #000000'><b>Banco</b></th>
                            <th style='border: solid 1px #000000'>Num Transfer</th>
                            <th style='border: solid 1px #000000'> Num Cheque </th>
                            <th style='border: solid 1px #000000'> Fecha Venc. </th>
                        </thead>
            ";
            pago.tiposPagoToSave.ForEach(tp => {
                if (tp.tipoPago == "Efectivo" || tp.tipoPago == "Transferencia") 
                {
                    msg += @"<tr>";
                    msg += "    <td style='border: solid 1px #000000'>" + tp.tipoPago + "</td>";
                    msg += "    <td style='border: solid 1px #000000'>USD " + tp.monto + "</td>";
                    msg += "    <td style='border: solid 1px #000000'>" + tp.bancoTxt + "</td>";
                    msg += "    <td style='border: solid 1px #000000'>" + tp.NumTransferencia + "</td>";
                    msg += "    <td style='border: solid 1px #000000'></td>";
                    msg += "    <td style='border: solid 1px #000000'></td>";
                    msg += "</tr>";
                }
                else {
                    tp.cheques.ForEach(cheque => {
                        msg += @"<tr>";
                        msg += "    <td style='border: solid 1px #000000'>" + tp.tipoPago + "</td>";
                        msg += "    <td style='border: solid 1px #000000'>USD " + cheque.monto + "</td>";
                        msg += "    <td style='border: solid 1px #000000'>" + cheque.bancoTxt + "</td>";
                        msg += "    <td style='border: solid 1px #000000'></td>";
                        msg += "    <td style='border: solid 1px #000000'>" + cheque.NumCheque + "</td>";
                        if(cheque.FechaVencimientoCheque!=null)
                            msg += "    <td style='border: solid 1px #000000'>" + cheque.FechaVencimientoCheque.ToString("yyyy-MM-dd") + "</td>";
                        msg += "</tr>";
                    });
                }
            });
            msg += "</table><br>";
            /*
             * En outlook no se carga, por eso mejor va como atachment la imgen del comprobante
            if (!string.IsNullOrEmpty(pago.photoComprobanteData))
            {
                msg += "<b>Comprobante Pago:</b> <br>";
                msg += string.Format(@"<img src=""data: image / png; base64, {0}""/>", pago.photoComprobanteData);
            }
            */
            msg += "<div><i><b>Nota: </b>Los pagos detallados en este correo están sujetos a revisión del departamento de cobranzas de James Brown Pharma</div></i>";
            
            var destinatarios = string.Format("{0}; {1}", conf.Default.correoPagos, correoVendedor) ;
            //para que no se envíe al cliente en ambiente de pruebas
            if (bddName.Equals("SBO_JBP_PROD")) //si es la base de datos de produccion
            {
                var correoCliente = SocioNegocioBusiness.GetCorreoByCodigo(pago.CodCliente);
                if (correoCliente != null && TechTools.Utils.ValidacionUtils.EmailValid(correoCliente))
                    destinatarios += "; " + correoCliente;
            }
            this.EnviarPorCorreo(destinatarios, titulo, msg, fotosPath);
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

        private string getNombreComprobantePago(string codCliente, int numFile)
        {
            var ms = string.Format("{0}_{1}_{2}.png",codCliente, DateTime.Now.ToString("yyyyMMddhhmm"),numFile.ToString());
            return ms;
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
}
