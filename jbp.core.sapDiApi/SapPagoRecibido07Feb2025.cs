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
    public class SapPagoRecibido07Feb2025:BaseSapObj
    {
        public SapPagoRecibido07Feb2025()
        {
            //this.Connect();
        }
        public string SafePago(PagosMsg me)
        {
            try
            {
                if(me==null)
                    return "No se han eviado pagos a procesar!!";
                if (me.tiposPagoToSave == null || me.tiposPagoToSave.Count == 0)
                    return "No se han enviado tipos de pago (Ej. Transferencias, cheques)!!";
                if (me.facturasAPagar == null || me.facturasAPagar.Count == 0)
                    return "No se han enviado facturas a pagar!!";
                /*
                 - El Objeto de entrada contiene uno o mas tipos de pago y una mas facturas  
                 - Un Documento de Pago SAP puede contener una o mas facturas, pero un solo tipo de pago (por reglas de JB)
                 - Se registran todos los pagos o ninguno incluidas NC si aplica (se lo maneja como una transacción atómica)
                */
                this.Company.StartTransaction();
                var requiereNC = this.RequiereCreacionNotaCredito(me);
                if (requiereNC)
                    registrarNotasCreditoProntoPago(me);

                setSaldosPorTipoPago(me);

                me.tiposPagoToSave.ForEach(tipoPago => {
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
                return ex.Message;
            }
            this.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
            return "ok";
        }

        private void DistribuirTipoPagoEnFacturas(TipoPagoMsg tipoPagoActual, List<DocCarteraMsg> facturasAPagar)
        {
            facturasAPagar.ForEach(factura => {
                //distribuyo el saldo del tipo de pago entre las facturas hasta lo que alcance a pagar
                if (tipoPagoActual.saldo > 0 && factura.toPay>0) { //si tengo saldo para pagar y la factura requiere de pago
                    if (factura.toPay >= tipoPagoActual.saldo)
                    {
                        /*
                         Cuando se asigna el total del saldo del tipo de pago a la factura
                         (Se paga el total o se hace un abono a la factura)
                        */
                        factura.pagado = tipoPagoActual.saldo;
                        factura.toPay -= factura.pagado;
                        tipoPagoActual.saldo = 0;
                    }
                    else {
                        /*
                         Se paga el total de la factura 
                        (hay mas en saldo del tipo de pago que el valor de la factura)
                        */
                        factura.pagado= factura.toPay;
                        factura.toPay = 0;
                        tipoPagoActual.saldo -= factura.pagado;
                    }
                    factura.PasoProcesoDistribucion = false;
                }
            });
        }

        private void RegistrarTipoPagoEnSap(PagosMsg me, TipoPagoMsg tipoPago)
        {
            dynamic pago=GetObjPagoSap(me, tipoPago);
            me.facturasAPagar.ForEach(factura =>
            {
                if (!factura.PasoProcesoDistribucion) {
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
                    factura.PasoProcesoDistribucion = true;
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
        
        //por borrar
        private void addDocumentoAlPago(ref dynamic pago, DocCarteraMsg documento, string codCliente)
        {
            pago.Invoices.DocEntry = documento.DocEntry;// el id de la factura o del cheque protestado
            if (documento.tipoDocumento == "Cheque Protestado")
            {
                //pago.Invoices.InvoiceType = SAPbobsCOM.BoRcptInvTypes.it_JournalEntry;//it_PaymentAdvice;
                //se empareja con el asiento contable
                JournalEntries oJE = this.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oJournalEntries);
                //oJE.GetByKey(documento.DocEntry);
                oJE.GetByKey(716980); //id del asiento contable
                for (int i = 0; i < oJE.Lines.Count; i++)
                {
                    oJE.Lines.SetCurrentLine(i);
                    if (oJE.Lines.ShortName == codCliente) //busco el asiento que corresponda con el cliente
                        pago.Invoices.DocLine = oJE.Lines.Line_ID;
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oJE);


            }
            else
            {//se asume que es factura

                pago.Invoices.InvoiceType = SAPbobsCOM.BoRcptInvTypes.it_Invoice;
                pago.Invoices.UserFields.Fields.Item("U_PORCENTAJE_PP").Value = documento.porcentajePP;
                pago.Invoices.UserFields.Fields.Item("U_DESCUENTO_PP").Value = documento.descuentoPP;
            }
            //pago.Invoices.DocumentStatus
            pago.Invoices.SumApplied = documento.pagado;
            pago.Invoices.Add();
        }

        public string SafePagoBk(PagosMsg pagoMe)
        {
            try
            {
                if (pagoMe.tiposPagoToSave == null || pagoMe.tiposPagoToSave.Count == 0)
                    return "No se han enviado tipos de pago!!";
                if(pagoMe.facturasAPagar==null || pagoMe.facturasAPagar.Count==0)
                    return "No se han enviado facturas a pagar!!";
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
                
                   Se ejecuta dentro de una transacción solo si hay NC ya que se afectan a pagos
                   Si hay un error en cualquiera de los dos documentos se hace un rollback

                   El Objeto de entrada pagoMe contiene uno o mas tipos de pago y uno mas facturas
                
                0. Registro Notas de crédito pronto pago (si aplica)
                1. Se asume que del front end las facturas ya llegan ordenadas por antiguedad
                2. Se asignan los pagos a las facturas
                */

                //0. Registro Notas de crédito pronto pago (si aplica)
                var requiereNC = this.RequiereCreacionNotaCredito(pagoMe);
                if (requiereNC)
                {
                    try
                    {
                        this.Company.StartTransaction();
                        registrarNotasCreditoProntoPago(pagoMe);
                    }
                    catch (Exception e)
                    {
                        this.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                        return e.Message;
                    }
                }
                
                //2.Se asignan los pagos a las facturas
                setSaldosPorTipoPago(me);
                var numPago = 0; //el numero de documento del tipo pago recibido que se inyectan en SAP
                var numFactura = 0;
                var tipoPagoActual = me.tiposPagoToSave[0];
                var documentoActual=me.facturasAPagar[0];
                var numFacturas = me.facturasAPagar.Count;
                dynamic pago=null;
                var registrarNuevoPago = true;
                // máximo en 20 iteraciones debe salir del bucle
                var numIteracion = 0;
                while (tipoPagoActual != null && tipoPagoActual.saldo > 0 && numPago<30 && numFactura<=numFacturas && numIteracion<20) {// se controla que no se de un bucle infinito
                    numIteracion++;
                    //TODO: Validar que no se registre 2 veces el mismo tipo de pago
                    if (registrarNuevoPago) {
                        setPago(ref pago, me, tipoPagoActual);
                        registrarNuevoPago=false;
                    }
                    tipoPagoActual.saldo = Math.Round(tipoPagoActual.saldo, 2);
                    documentoActual.saldo = Math.Round(documentoActual.saldo, 2);
                    if (tipoPagoActual.saldo <= documentoActual.saldo)
                    {
                        documentoActual.pagado = tipoPagoActual.saldo;
                        documentoActual.saldo -= tipoPagoActual.saldo;
                        tipoPagoActual.saldo = 0;
                    }
                    else { 
                        tipoPagoActual.saldo-=documentoActual.saldo;
                        documentoActual.pagado=documentoActual.saldo;
                        documentoActual.saldo = 0;
                    }
                    addDocumentoAlPago(ref pago, documentoActual, me.CodCliente);
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
                    documentoActual.saldo=Math.Round(documentoActual.saldo,2); //para evitar valore < 1 centavo
                    if (documentoActual.saldo == 0) {
                        numFactura++;
                        if(numFactura < me.facturasAPagar.Count)
                            documentoActual=me.facturasAPagar[numFactura];
                    }
                }
                if (requiereNC)
                    this.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                return ms;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        //borrar esta función
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
                    if (tipoPago.fechaTransferencia != null) {
                        pago.TransferDate = tipoPago.fechaTransferencia;
                        pago.DocDate = tipoPago.fechaTransferencia;
                    }
                        
                    break;
            }
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
            var sapNc = new SapNotaCredito();
            sapNc.Company = this.Company;
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
        ~SapPagoRecibido07Feb2025()
        {
            //cuando se destruye el objeto
            this.Disconnect();
        }
    }
}
