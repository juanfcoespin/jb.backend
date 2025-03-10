using System.Collections.Generic;
using System;

namespace jbp.msg.sap
{
    public class PagosMsg: ICloneable
    {
        public double totalAPagar { get; set; }
        public double GetTotalPagado() {
                double ms = 0;
                if (this.tiposPagoToSave!=null && this.tiposPagoToSave.Count > 0)
                {
                    this.tiposPagoToSave.ForEach(tp => {
                        if (tp.cheques != null && tp.cheques.Count > 0)
                        {
                            tp.cheques.ForEach(cheque => ms += cheque.monto);
                        }
                        else
                            ms += tp.monto;
                        
                    });
                }
                return ms;
        }
        public List<DocCarteraMsg> facturasAPagar { get; set; }
        public List<TipoPagoMsg> tiposPagoToSave { get; set; }
        public List<string> fotosComprobantes { get; set; }
        public string CodCliente { get; set; }
        public string client { get; set; }
        public string comment { get; set; }
        public string Vendedor { get; set; }
        public string fechaImpresion { get; set; }
        public string numRecibo { get; set; }


        public object Clone()
        {
            var ms = (PagosMsg)MemberwiseClone();
            ms.facturasAPagar = new List<DocCarteraMsg>();
            this.facturasAPagar.ForEach(f => {
                ms.facturasAPagar.Add((DocCarteraMsg)f.Clone());
            });
            ms.tiposPagoToSave = new List<TipoPagoMsg>();
            this.tiposPagoToSave.ForEach(tipoPago => {
                ms.tiposPagoToSave.Add((TipoPagoMsg)tipoPago.Clone());
            });
            return ms;
        }
    }
    public class TipoPagoMsg: ICloneable
    {
        public dynamic fechaTransferencia;

        public string tipoPago { get; set; }
        public double monto { get; set; }
        public List<ChequeMsg> cheques { get; set; }

        //Transferencia
        public string CodigoCuentaJB { get; set; }
        public int NumTransferencia { get; set; }
        public string bancoTxt { get; set; }
        public double saldo { get; set; }

        public TipoPagoMsg() { 
            this.cheques = new List<ChequeMsg>();
        }
        public object Clone()
        {
            return (TipoPagoMsg)MemberwiseClone();
        }
    }

    public class ChequeMsg
    {
        public dynamic monto;

        //cheque
        public string CodigoCuentaCheque { get; set; }
        public string FechaVencimientoChequeStr { get; set; }
        public DateTime FechaVencimientoCheque
        {
            get
            {
                //FechaVencimientoChequeStr: "2021-06-19T21:13:52.289-05:00"
                if (this.FechaVencimientoChequeStr != null && this.FechaVencimientoChequeStr.Length >= 10)
                {
                    //2021-06-19
                    var strFecha = this.FechaVencimientoChequeStr.Substring(0, 10);
                    var mFecha = strFecha.Split(new char[] { '-' });
                    if (mFecha.Length >= 3)
                        return new DateTime(Convert.ToInt32(mFecha[0]), Convert.ToInt32(mFecha[1]), Convert.ToInt32(mFecha[2]));
                }
                return DateTime.MinValue;

            }
        }
        public string CodigoBanco { get; set; }
        public int NumCheque { get; set; }
        public string Posfechado { get; set; }
        public string bancoTxt { get; set; }
    }
    public class DocCarteraMsg: ICloneable
    {
        public double saldo;
        public string comentarioCobroPorExcepcion;

        public decimal total { get; set; }
        public double toPay { get; set; }
        public string numDoc { get; set; }
        public string date { get; set; }
        public string dueDate { get; set; }
        public int DocEntry { get; set; }
        public double pagado { get; set; }
        public double toPayMasProntoPago { get; set; }
        public int porcentajePP { get; set; }
        public int folioNum { get; set; }
        public double descuentoPP { get; set; }
        public DatosRelacionadosFacturaPagoMsg DatosAdicionales { get; set; }
        public string tipoDocumento { get; set; }
        public string CodCliente { get; set; }
        public bool PasoProcesoDistribucion { get; set; }
        public double valorPagado { get; set; }
        public int IdFactura { get; set; }

        public object Clone()
        {
            return (DocCarteraMsg)MemberwiseClone();
        }
    }

    public class AsignacionPagosMsg {
        public TipoPagoMsg TipoPago { get; set; }
        public List<DocCarteraMsg> facturasAPagar { get; set; }
    }
    public class ValorPagadoMsg
    {
        public decimal Valor { get; set; }
        public string Fecha { get; set; }
        public string DocNum { get; set; }
    }
}
