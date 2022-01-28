using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace jbp.msg.sap
{
    public class CarteraMsg
    {
        public string CodSocioNegocio { get; set; }
        public string TipoDocumento { get; set; }
        public string Vendedor { get; set; }
        public string NumFolio { get; set; }
        public string FechaDocumento { get; set; }
        public decimal TotalDocumento { get; set; }
        public string FechaVencimiento { get; set; }
        public int DiasVencido { get; set; }
        public decimal TotalPago { get; set; }
        public decimal SaldoVencido { get; set; }
        public string RangoDiasVencido { get; set; }
        public int OrdenRango { get; set; }
        public int OrdenTipoDocumento { get; set; }
        public int DocNum { get; set; }
        public List<RetencionMsg> Retenciones { get; set; }
        public List<ValorPagadoMsg> Pagos { get; set; }
    }
}
