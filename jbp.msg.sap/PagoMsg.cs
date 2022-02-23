using System.Collections.Generic;
using System;

namespace jbp.msg.sap
{
    public class PagoMsg: ICloneable
    {
        public double totalAPagar { get; set; }
        public double totalPagado {
            get
            {
                double _totalPagado = 0;
                if (tiposPago!=null && tiposPago.Count > 0)
                {
                    this.tiposPago.ForEach(tp => _totalPagado += tp.monto);
                }
                return _totalPagado;
            }
        }
        public List<DocCarteraMsg> facturasAPagar { get; set; }
        public List<TipoPagoMsg> tiposPago { get; set; }
        public string photoComprobanteData { get; set; }
        public string CodCliente { get; set; }
        public string client { get; set; }
        public string comment { get; set; }
        public string Vendedor { get; set; }

        public object Clone()
        {
            var ms = (PagoMsg)MemberwiseClone();
            ms.facturasAPagar = new List<DocCarteraMsg>();
            this.facturasAPagar.ForEach(f => {
                ms.facturasAPagar.Add((DocCarteraMsg)f.Clone());
            });
            ms.tiposPago = new List<TipoPagoMsg>();
            this.tiposPago.ForEach(tipoPago => {
                ms.tiposPago.Add((TipoPagoMsg)tipoPago.Clone());
            });
            return ms;
        }
    }
    public class TipoPagoMsg: ICloneable
    {
        public string tipoPago { get; set; }
        public double monto { get; set; }
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

        //Transferencia
        public string CodigoCuentaJB { get; set; }
        public int NumTransferencia { get; set; }
        public string bancoTxt { get; set; }

        public object Clone()
        {
            return (TipoPagoMsg)MemberwiseClone();
        }
    }

    public class DocCarteraMsg: ICloneable
    {
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
    }
}
