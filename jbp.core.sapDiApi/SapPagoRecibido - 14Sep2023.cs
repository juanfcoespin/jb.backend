using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jbp.msg.sap;
using TechTools.Utils;

namespace jbp.core.sapDiApi
{
    public class SapPagoRecibidoBk:BaseSapObj
    {
        public SapPagoRecibidoBk()
        {
            //this.Connect();
        }
        public string Add(PagosMsg pagoMe)
        {
            try
            {
                
                var sapNc = new SapNotaCredito();
                sapNc.Company = this.Company;
                this.Company.StartTransaction();
                registrarNotasCreditoProntoPago(pagoMe, sapNc);

                /*
                   Se copia el objeto porque fuera de esta función
                   se utiliza la referencia original para generar el correo electrónico
                   de resumen del pago y hay ciertas modificaciones en las NC Pronto Pago
                */
                var me = (PagosMsg)pagoMe.Clone();
                var ms = "ok";
                /*
                   Un Documento de Pago SAP puede contener una o mas facturas, 
                   pero un solo tipo de pago (por reglas de JB)
                
                   Se ejecuta dentro de una transacción ya que se afectan a pagos y notas de crédito
                   Si hay un error en cualquiera de los dos documentos se hace un rollback
                
                0. Registro Notas de crédito pronto pago (si aplica)
                1. Ordeno Facturas por atiguedad (se asume que del front end ya llegan ordenadas)
               
                */

                foreach (var tipoPago in me.tiposPagoToSave)
                {
                    var pago = this.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPaymentsDrafts);
                    pago.DocObjectCode = SAPbobsCOM.BoPaymentsObjectType.bopot_IncomingPayments;
                    pago.CardCode = me.CodCliente;
                    pago.DocDate = DateTime.Now;
                    pago.DocType = SAPbobsCOM.BoRcptTypes.rCustomer;
                    //pago.DocType = SAPbobsCOM.BoRcptTypes.;
                    pago.Remarks = me.comment;
                    
                    switch (tipoPago.tipoPago)
                    {
                        case "Efectivo":
                            pago.CashSum = tipoPago.monto;
                            break;
                        case "Cheque":
                            addCheques(tipoPago, pago);
                            break;
                        case "ChequePosfechado":
                            pago.Series = 133; //Serie CHEQUE-POS
                            addCheques(tipoPago, pago);
                            break;
                        case "Transferencia":
                            pago.TransferReference = tipoPago.NumTransferencia;
                            pago.TransferAccount = tipoPago.CodigoCuentaJB;
                            pago.TransferSum = tipoPago.monto;
                            break;
                    }
                    /*
                    3. Asignacion de pago a facturas
                       caso 1: se paga todo el saldo de la factura
                       caso 2: se paga un abono a la factura
                    */
                    var line = 0;
                    double saldo = tipoPago.monto; //para calcular el monto a pagar a las facturas
                    foreach (var factura in me.facturasAPagar)
                    {
                        if (saldo > 0 && factura.toPayMasProntoPago > 0 && factura.DocEntry > 0)
                        {
                            pago.Invoices.DocEntry = factura.DocEntry;
                            
                            pago.Invoices.InvoiceType = SAPbobsCOM.BoRcptInvTypes.it_Invoice;
                            pago.Invoices.UserFields.Fields.Item("U_PORCENTAJE_PP").Value = factura.porcentajePP;
                            pago.Invoices.UserFields.Fields.Item("U_DESCUENTO_PP").Value = factura.descuentoPP;
                            line++;
                            if (saldo >= factura.toPayMasProntoPago)// caso 1: se paga todo el saldo de la factura
                            {
                                factura.pagado = factura.toPayMasProntoPago;
                            }
                            else
                            {// caso 2: se paga un abono a la factura
                                factura.pagado = saldo;
                            }
                            factura.toPayMasProntoPago = factura.toPayMasProntoPago - factura.pagado;
                            saldo -= factura.pagado;// se actualiza el saldo para la siguiente factura
                            pago.Invoices.SumApplied = factura.pagado;
                            pago.Invoices.Add();
                        }
                    }
                    var error = pago.Add();//registro un documento de pago en SAP
                    if (error != 0) // si hay error en el registro del pago
                    {
                        ms = "Error: " + this.Company.GetLastErrorDescription();
                        return ms;
                    }

                }
                this.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                return ms;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private void addCheques(TipoPagoMsg tipoPago, dynamic pago)
        {
            tipoPago.cheques.ForEach(cheque => {
                pago.Checks.CheckNumber = cheque.NumCheque;
                pago.Checks.CheckSum = cheque.monto;
                //pago.Checks.AccounttNum = cheque.CodigoCuentaCheque;
                pago.Checks.BankCode = cheque.CodigoBanco;
                pago.Checks.DueDate = cheque.FechaVencimientoCheque;
                pago.Checks.UserFields.Fields.Item("U_POSTFECHADO").Value = cheque.Posfechado;
                pago.Checks.Add();
                tipoPago.monto += cheque.monto;
            });
        }

        private void registrarNotasCreditoProntoPago(PagosMsg me, SapNotaCredito sapNc)
        {
            me.facturasAPagar.ForEach(factura => {
                if (factura.porcentajePP > 0)
                {
                    factura.descuentoPP = factura.toPay - factura.toPayMasProntoPago;
                    //se registra la nota de credito por pronto pago
                    var ncPP = getNcProntoPagoFromFactura(factura, me);
                    var respNc = sapNc.AddNcProntoPago(ncPP);
                    if (respNc != "ok")
                        throw new Exception(respNc);
                }
            });
        }
        private NotaCreditoPPMsg getNcProntoPagoFromFactura(DocCarteraMsg factura, PagosMsg pago)
        {

            var comentario = string.Format("Porcentaje Desc. PP: {0}%, valor: {1} Factura: {2}, Total Factura: {3}",
                factura.porcentajePP, factura.descuentoPP, factura.numDoc, factura.total);
            //pago.comment += comentario;
            var ms = new NotaCreditoPPMsg
            {
                CodCliente = pago.CodCliente,
                Comentario = comentario,
                TipoDescPP = eNcPPType.Veterinario,
                FolioNumFacturaRelacionada = factura.folioNum,
                TotalNC = factura.descuentoPP
            };
            return ms;
        }
        ~SapPagoRecibidoBk()
        {
            //cuando se destruye el objeto
            this.Disconnect();
        }
    }
}
