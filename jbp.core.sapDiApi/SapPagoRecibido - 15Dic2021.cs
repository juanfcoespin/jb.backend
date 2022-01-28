using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jbp.msg.sap;

namespace jbp.core.sapDiApi
{
    public class SapPagoRecibido15Dic2021:BaseSapObj
    {
        public SapPagoRecibido15Dic2021()
        {
            //this.Connect();
        }
        public string Add(PagoMsg me)
        {
            
            var ms = "ok";
            //var pago= this.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oIncomingPayments);
            
            //esto es equivalente al guardado preeliminar o guardarlo como borrador en SAP para que no se disminuya la cartera
            var pago = this.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPaymentsDrafts);

            pago.DocObjectCode= SAPbobsCOM.BoPaymentsObjectType.bopot_IncomingPayments;
            pago.CardCode = me.CodCliente;
            pago.DocDate = DateTime.Now;
            pago.DocType = SAPbobsCOM.BoRcptTypes.rCustomer;
            //pago.DocType = SAPbobsCOM.BoRcptTypes.;
            pago.Remarks = me.comment;
            
            //en un solo pago se pueden contener varios tipos de pago
            //Ej. 3500 desgregados de la siguiente manera: 2000 transferencia, 1000 cheque, 500 efectivo
            me.tiposPago.ForEach(tp => {
                switch (tp.tipoPago)
                {
                    case "Efectivo":
                        pago.CashSum = tp.monto;
                        break;
                    case "Cheque":
                        pago.Checks.CheckNumber = tp.NumCheque;
                        pago.Checks.CheckSum = tp.monto;
                        pago.Checks.AccounttNum = tp.CodigoCuentaCheque;
                        pago.Checks.BankCode = tp.CodigoBanco;
                        pago.Checks.DueDate = tp.FechaVencimientoCheque;
                        pago.Checks.UserFields.Fields.Item("U_POSTFECHADO").Value = tp.Posfechado;
                        pago.Checks.Add();
                        break;
                    case "Transferencia":
                        pago.TransferReference = tp.NumTransferencia;
                        pago.TransferAccount = tp.CodigoCuentaJB;
                        pago.TransferSum = tp.monto;
                        break;
                }
            });
            var line = 0;
            double saldo = me.totalPagado; //para calcular el saldo de la ultima factura
            me.facturasAPagar.ForEach(factura =>
            {
                if (factura.DocEntry > 0)
                {
                    pago.Invoices.DocEntry = factura.DocEntry;
                    pago.Invoices.InvoiceType = SAPbobsCOM.BoRcptInvTypes.it_Invoice;
                    line++;
                    if (saldo >= factura.toPay)
                    {
                        factura.pagado = factura.toPay;
                        saldo -= factura.toPay;
                    }
                    else{ //se paga un abono a la ultima factura
                        factura.pagado = saldo;
                    }
                    pago.Invoices.SumApplied = factura.pagado;
                    pago.Invoices.Add();
                }
            });
            var error = pago.Add();
            if (error != 0)
            {
                ms= "Error: "+this.Company.GetLastErrorDescription();
            }
            return ms;
        }

       

        ~SapPagoRecibido15Dic2021()
        {
            //cuando se destruye el objeto
            this.Disconnect();
        }
    }
}
