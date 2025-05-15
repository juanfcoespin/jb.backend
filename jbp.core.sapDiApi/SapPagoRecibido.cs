using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jbp.msg;
using System.Xml.Linq;
using jbp.msg.sap;
using SAPbobsCOM;
using TechTools.Utils;

namespace jbp.core.sapDiApi
{
    public class SapPagoRecibido:BaseSapObj
    {
        public event Action<string> OnMessageArrived;
        public void Notify(string message)
        {
            if (string.IsNullOrEmpty(message))
                return;
            OnMessageArrived?.Invoke(message);
        }

        public SapPagoRecibido()
        {
            //this.Connect();
        }
        public void SafePago(PagosMsg me)
        {
            try
            {
                if(me==null)
                    throw new Exception("No se han eviado pagos a procesar!!");
                if (me.tiposPagoToSave == null || me.tiposPagoToSave.Count == 0)
                    throw new Exception("No se han enviado tipos de pago (Ej. Transferencias, cheques)!!");
                if (me.facturasAPagar == null || me.facturasAPagar.Count == 0)
                    throw new Exception("No se han enviado facturas a pagar!!");

                /*
                 - El Objeto de entrada contiene uno o mas tipos de pago y una mas facturas  
                 - Un Documento de Pago SAP puede contener una o mas facturas, pero un solo tipo de pago (por reglas de JB)
                 - Se registran todos los pagos o ninguno incluidas NC si aplica (se lo maneja como una transacción atómica)
                */
                Notify("Iniciando Transacción");
                this.Company.StartTransaction();
                var requiereNC = this.RequiereCreacionNotaCredito(me);
                if (requiereNC)
                    registrarNotasCreditoProntoPago(me);

                setSaldosPorTipoPago(me);

                me.tiposPagoToSave.ForEach(tipoPago => {
                    Notify("Distribuyendo Pagos en Facturas");
                    DistribuirTipoPagoEnFacturas(tipoPago, me.facturasAPagar);
                    // Una vez distribuidos los pagos del tipo de pago a las facturas:
                    RegistrarTipoPagoEnSap(me,tipoPago);
                });
            }
            catch (Exception ex)
            {
                try
                {
                    this.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                }
                catch { }
                throw new Exception(ex.Message);
            }
            this.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
            Notify("Transacción Completada");
        }

        private void DistribuirTipoPagoEnFacturas(TipoPagoMsg tipoPagoActual, List<DocCarteraMsg> facturasAPagar)
        {
            facturasAPagar.ForEach(factura => {
                //distribuyo el saldo del tipo de pago entre las facturas hasta lo que alcance a pagar
                if (tipoPagoActual.saldo > 0 && factura.toPayMasProntoPago > 0) { //si tengo saldo para pagar y la factura requiere de pago
                    if (factura.toPayMasProntoPago >= tipoPagoActual.saldo)
                    {
                        /*
                         Cuando se asigna el total del saldo del tipo de pago a la factura
                         (Se paga el total o se hace un abono a la factura)
                        */
                        factura.pagado = tipoPagoActual.saldo;
                        
                        factura.toPayMasProntoPago -= factura.pagado;
                        factura.toPayMasProntoPago = Math.Round(factura.toPayMasProntoPago, 2);   
                        tipoPagoActual.saldo = 0;
                    }
                    else {
                        /*
                         Se paga el total de la factura 
                        (hay mas en saldo del tipo de pago que el valor de la factura)
                        */
                        factura.pagado= factura.toPayMasProntoPago;
                        factura.toPayMasProntoPago = 0;
                        tipoPagoActual.saldo -= factura.pagado;
                        tipoPagoActual.saldo = Math.Round(tipoPagoActual.saldo, 2);
                    }
                    factura.valorPagado += factura.pagado;
                    factura.valorPagado = Math.Round(factura.valorPagado, 2);
                    factura.PasoProcesoDistribucion = true;
                }
            });
        }

        private void RegistrarTipoPagoEnSap(PagosMsg me, TipoPagoMsg tipoPago)
        {
            dynamic pago=GetObjPagoSap(me, tipoPago);
            me.facturasAPagar.ForEach(factura =>
            {
                if (factura.PasoProcesoDistribucion) {
                    pago.Invoices.DocEntry = factura.DocEntry;// el id de la factura o del cheque protestado
                    if (factura.tipoDocumento == "Cheque Protestado")
                    {
                        //pago.Invoices.InvoiceType = SAPbobsCOM.BoRcptInvTypes.it_JournalEntry;//it_PaymentAdvice;
                        //se empareja con el asiento contable
                        JournalEntries oJE = this.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oJournalEntries);
                        //oJE.GetByKey(documento.DocEntry);
                        oJE.GetByKey(716980); //id del asiento contable
                        for (int i = 0; i < oJE.Lines.Count; i++)
                        {
                            oJE.Lines.SetCurrentLine(i);
                            if (oJE.Lines.ShortName == me.CodCliente) //busco el asiento que corresponda con el cliente
                                pago.Invoices.DocLine = oJE.Lines.Line_ID;
                        }
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(oJE);


                    }
                    else
                    {//se asume que es factura

                        pago.Invoices.InvoiceType = SAPbobsCOM.BoRcptInvTypes.it_Invoice;
                        pago.Invoices.UserFields.Fields.Item("U_PORCENTAJE_PP").Value = factura.porcentajePP;
                        pago.Invoices.UserFields.Fields.Item("U_DESCUENTO_PP").Value = factura.descuentoPP;
                    }
                    //pago.Invoices.DocumentStatus
                    pago.Invoices.SumApplied = factura.pagado;
                    pago.Invoices.Add();
                    if(factura.toPay>0) // para que se continúe pagando con los otros tipos de pago
                        factura.PasoProcesoDistribucion = false;
                }
            });
            var error = pago.Add();//registro un documento de pago en SAP
            if (error != 0) // si hay error en el registro del pago
            {
                var ms = "Error: " + this.Company.GetLastErrorDescription();
                throw new Exception(ms);
            }
        }
        private dynamic GetObjPagoSap(PagosMsg me, TipoPagoMsg tipoPago)
        {
            dynamic pago = this.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPaymentsDrafts);
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
                    pago.CashAccount = "_SYS00000000687"; //TRANSITORIA CAJA EFECTIVO
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
                    if (tipoPago.fechaTransferencia != null)
                    {
                        pago.TransferDate = tipoPago.fechaTransferencia;
                        pago.DocDate = tipoPago.fechaTransferencia;
                    }

                    break;
            }
            return pago;
        }

        private void setSaldosPorTipoPago(PagosMsg me)
        {
            establecerTotalPagadoCheques(me);
            // en tipos de pago
            me.tiposPagoToSave.ForEach(tp => tp.saldo = tp.monto);
            //en facturas
            me.facturasAPagar.ForEach(factura => factura.saldo = factura.toPayMasProntoPago);
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

        private void registrarNotasCreditoProntoPago(PagosMsg me)
        {
            Notify("Registrando Notas de Crédito por pronto pago");
            var sapNc = new SapNotaCredito();
            sapNc.Company = this.Company;
            me.facturasAPagar.ForEach(factura => {
                if (factura.porcentajePP > 0)
                {
                    factura.descuentoPP = factura.toPay - factura.toPayMasProntoPago;
                    //se registra la nota de credito por pronto pago
                    var ncPP = getNcProntoPagoFromFactura(factura, me);
                    Notify("Aplicando "+ ncPP.TotalNC.ToString("C2") + " a la factura " + factura.numDoc);
                    var respNc = sapNc.AddNcProntoPago(ncPP);
                    if (respNc != "ok")
                        throw new Exception(respNc);
                    Notify("Nota de crédito por pronto pago registrada correctamente: ");
                }
            });
        }
        private bool RequiereCreacionNotaCredito(PagosMsg me) {
            foreach (var factura in me.facturasAPagar) {
                if (factura.porcentajePP > 0)
                    return true;
            }
            return false;
                
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
