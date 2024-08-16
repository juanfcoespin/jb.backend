using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jbp.msg.sap;
using TechTools.Utils;

namespace jbp.core.sapDiApi
{
    public class SapPagoRecibido:BaseSapObj
    {
        public SapPagoRecibido()
        {
            //this.Connect();
        }
        public string SafePagos(PagosMsg pagosMe)
        {
            try
            {
                if (pagosMe.tiposPagoToSave == null || pagosMe.tiposPagoToSave.Count == 0)
                    return "No se han enviado tipos de pago!!";
                if(pagosMe.facturasAPagar==null || pagosMe.facturasAPagar.Count==0)
                    return "No se han enviado facturas a pagar!!";
                /*
                   Se copia el objeto porque fuera de esta función
                   se utiliza la referencia original para generar el correo electrónico
                   de resumen del pago y hay ciertas modificaciones en las NC Pronto Pago
                */
                var me = (PagosMsg)pagosMe.Clone();
                var ms = "ok";
                /*
                   Un Documento de Pago SAP puede contener una o mas facturas, 
                   pero un solo tipo de pago (por reglas de JB)
                
                   Se ejecuta dentro de una transacción ya que se afectan a pagos y notas de crédito
                   Si hay un error en cualquiera de los dos documentos se hace un rollback
                
                0. Registro Notas de crédito pronto pago (si aplica)
                1. Se asume que del front end las facturas ya llegan ordenadas por antiguedad
                2. Se asignan los pagos a las facturas
                */

                //0. Registro Notas de crédito pronto pago (si aplica)
                var sapNc = new SapNotaCredito();
                sapNc.Company = this.Company;
                this.Company.StartTransaction();
                try
                {
                    registrarNotasCreditoProntoPago(pagosMe, sapNc);
                }
                catch (Exception e) {
                    this.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                    return e.Message;
                }
                
                //2.Se asignan los pagos a las facturas
                establecerTotalPagadoCheques(me);
                setSaldos(me);
                var numPago = 0; //el numero de documento del tipo pago recibido que se inyectan en SAP
                var numFactura = 0;
                var tipoPagoActual = me.tiposPagoToSave[0];
                var facturaActual=me.facturasAPagar[0];
                dynamic pago=null;
                var registrarNuevoPago = true;
                while (tipoPagoActual != null && tipoPagoActual.saldo > 0 && numPago<30) {// se controla que no se de un bucle infinito
                    //TODO: Validar que no se registre 2 veces el mismo tipo de pago
                    if (registrarNuevoPago) {
                        setPago(ref pago, me, tipoPagoActual);
                        registrarNuevoPago=false;
                    }
                    if (tipoPagoActual.saldo <= facturaActual.saldo)
                    {
                        facturaActual.pagado = tipoPagoActual.saldo;
                        facturaActual.saldo -= tipoPagoActual.saldo;
                        tipoPagoActual.saldo = 0;
                    }
                    else { 
                        tipoPagoActual.saldo-=facturaActual.saldo;
                        facturaActual.pagado=facturaActual.saldo;
                        facturaActual.saldo = 0;
                    }
                    addFacturaAlPago(ref pago, facturaActual);
                    if (tipoPagoActual.saldo == 0) {
                        var error=addPagoRecibido(ref pago);
                        if(error != null) 
                            return error;
                        registrarNuevoPago = true;
                        numPago++;
                        if (numPago < me.tiposPagoToSave.Count)
                            tipoPagoActual = me.tiposPagoToSave[numPago];
                        else
                            tipoPagoActual = null; //para que salga del bucle
                    }
                    if (facturaActual.saldo == 0) {
                        numFactura++;
                        if(numFactura < me.facturasAPagar.Count)
                            facturaActual=me.facturasAPagar[numFactura];
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

        private string addPagoRecibido(ref dynamic pago)
        {
            var error = pago.Add();//registro un documento de pago en SAP
            if (error != 0) // si hay error en el registro del pago
            {
                var ms = "Error: " + this.Company.GetLastErrorDescription();
                return ms;
            }
            return null;
        }

        private void setPago(ref dynamic pago, PagosMsg me, TipoPagoMsg tipoPago)
        {
            pago = this.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPaymentsDrafts);
            pago.DocObjectCode = SAPbobsCOM.BoPaymentsObjectType.bopot_IncomingPayments;
            pago.CardCode = me.CodCliente;
            pago.DocDate = DateTime.Now;
            pago.DocType = SAPbobsCOM.BoRcptTypes.rCustomer;
            //pago.DocType = SAPbobsCOM.BoRcptTypes.;
            pago.Remarks = me.comment;
            pago.UserFields.Fields.Item("U_jbpNroReciboCobro").Value = me.numRecibo;

            switch (tipoPago.tipoPago)
            {
                case "Efectivo":
                    pago.CashSum = tipoPago.monto;
                    pago.CashAccount = "1101011100";
                    break;
                case "Cheque":
                    addCheques(tipoPago, pago, me.numRecibo);
                    break;
                case "ChequePosfechado":
                    pago.Series = 21; //Serie CHEQ_POS
                    addCheques(tipoPago, pago, me.numRecibo);
                    break;
                case "Transferencia":
                    pago.TransferReference = tipoPago.NumTransferencia;
                    pago.TransferAccount = tipoPago.CodigoCuentaJB;
                    pago.TransferSum = tipoPago.monto;
                    pago.CounterReference = tipoPago.NumTransferencia;
                    if (tipoPago.fechaTransferencia != null) {
                        pago.TransferDate = tipoPago.fechaTransferencia;
                        pago.DocDate = tipoPago.fechaTransferencia;
                    }
                        
                    break;
            }
        }

        private void addFacturaAlPago(ref dynamic pago, DocCarteraMsg factura)
        {
            pago.Invoices.DocEntry = factura.DocEntry;
            pago.Invoices.InvoiceType = SAPbobsCOM.BoRcptInvTypes.it_Invoice;
            pago.Invoices.UserFields.Fields.Item("U_PORCENTAJE_PP").Value = factura.porcentajePP;
            pago.Invoices.UserFields.Fields.Item("U_DESCUENTO_PP").Value = factura.descuentoPP;
            pago.Invoices.SumApplied = factura.pagado;
            pago.Invoices.Add();
        }

        private void setSaldos(PagosMsg me)
        {
            // en tipos de pago
            me.tiposPagoToSave.ForEach(tp => tp.saldo = tp.monto);

            //en facturas
            me.facturasAPagar.ForEach(factura => factura.saldo = factura.toPay);
        }

        private void establecerTotalPagadoCheques(PagosMsg me)
        {
            me.tiposPagoToSave.ForEach(tp => { 
                tp.cheques.ForEach(cheque => {
                    tp.monto += cheque.monto;
                    tp.monto = Math.Round(tp.monto, 2); 
                });
            });
        }

        private void addCheques(TipoPagoMsg tipoPago, dynamic pago, string numReciboCobro)
        {
            tipoPago.cheques.ForEach(cheque => {
                if (cheque.monto > 0) {
                    pago.Checks.CheckNumber = cheque.NumCheque;
                    pago.Checks.CheckSum = cheque.monto;
                    //pago.Checks.AccounttNum = cheque.CodigoCuentaCheque;
                    pago.Checks.BankCode = cheque.CodigoBanco;
                    pago.Checks.DueDate = cheque.FechaVencimientoCheque;
                    pago.Checks.UserFields.Fields.Item("U_POSTFECHADO").Value = cheque.Posfechado;
                    pago.Checks.FiscalID = numReciboCobro;
                    pago.Checks.Add();
                }
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
                TotalNC = factura.descuentoPP,
                DatosAdicionalesFactura = factura.DatosAdicionales
            };
            return ms;
        }
        ~SapPagoRecibido()
        {
            //cuando se destruye el objeto
            this.Disconnect();
        }
    }
}
