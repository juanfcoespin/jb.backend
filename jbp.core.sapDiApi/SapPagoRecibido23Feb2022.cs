﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jbp.msg.sap;
using TechTools.Utils;

namespace jbp.core.sapDiApi
{
    public class SapPagoRecibido_23Feb2022:BaseSapObj
    {
        public SapPagoRecibido_23Feb2022()
        {
            //this.Connect();
        }
        public string Add(PagoMsg pagoMe)
        {
            /*
             Se copia el objeto porque fuera de esta función
             se utiliza la referencia original para generar el correo electrónico
             de resumen del pago 
            */


            var me = (PagoMsg)pagoMe.Clone();
            var ms = "ok";
          
            /*
               Un Documento de Pago SAP puede contener una o mas facturas, 
               pero un solo tipo de pago (por reglas de JB)
             
            1. Ordeno Tipos de pago por monto de mayor a menor
            2. Ordeno Facturas por monto de mayor a menor
            3. Empiezo a asignar los pagos de la siguiente manera:
            Ej:
                Pagos:
                 300 usd (efectivo)
                 150 usd (cheque)
                 100 usd (transferencia)
                Facturas:
                 Fact1 300 usd
                 Fact2 250 usd
                            
            Documentos de Pago en SAP:
                1: 300 usd (efectivo) -> Fact1 saldo 0
                2: 150 usd (cheque) -> Fact2 saldo 100
                3: 100 usd (transferencia) -> Fact2 saldo 0
            */

            //1.Ordeno Tipos de pago por monto de mayor a menor
            me.tiposPago = me.tiposPago.OrderByDescending(tp => tp.monto).ToList();

            //2.Ordeno Facturas por monto de mayor a menor
            me.facturasAPagar = me.facturasAPagar.OrderByDescending(f => f.toPayMasProntoPago).ToList();

            foreach(var tipoPago in me.tiposPago) {
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
                        pago.Checks.CheckNumber = tipoPago.NumCheque;
                        pago.Checks.CheckSum = tipoPago.monto;
                        pago.Checks.AccounttNum = tipoPago.CodigoCuentaCheque;
                        pago.Checks.BankCode = tipoPago.CodigoBanco;
                        pago.Checks.DueDate = tipoPago.FechaVencimientoCheque;
                        pago.Checks.UserFields.Fields.Item("U_POSTFECHADO").Value = tipoPago.Posfechado;
                        pago.Checks.Add();
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
                foreach(var factura in me.facturasAPagar)
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
            return ms;
        }

        

        ~SapPagoRecibido_23Feb2022()
        {
            //cuando se destruye el objeto
            this.Disconnect();
        }
    }
}