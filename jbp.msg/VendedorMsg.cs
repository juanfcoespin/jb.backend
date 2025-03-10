using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbp.msg
{
    public class VendedorConCobrosMsg
    {
        public string Nombre { get; set; }
        public List<CobroMsg> Cobros { get; set; }
        public string Email{ get; set; }

        public VendedorConCobrosMsg() { 
            this.Cobros = new List<CobroMsg>(); 
        }  
    }
    public class CobroMsg
    {
        public string Id { get; set; }
        public string Fecha { get; set; }
        public string Cliente { get; set; }
        public decimal TotalCobrado { get; set; }
        public List<PagoMsg>Pagos{ get; set; }
        public string Estado { get; set; }

        public CobroMsg()
        {
            this.Pagos=new List<PagoMsg>();
        }
    }
    public class PagoMsg
    {
        // tranferencia, cheque o efectivo
        public string CtaBancoJB { get; set; }
        public string BancoCheque { get; set; }
        public string TipoPago { get; set; }
        public string NumTransferencia { get; set; }
        public string FechaTransferencia { get; set; }
        public decimal MontoCheque { get; set; }
        public string FechaVencimientoCheque { get; set; }
        public string NroCheque { get; set; }
    }
    public enum eTipoOperacionVendedor
    {
        insert=1,
        update=2
    }
    public class VendedorPtkMsg
    {
        public int operacion { get; set; }
        public string nombreVendedor { get; set; }
        public string usuarioVendedor { get; set; }
        public string clave { get; set; }
        public string correo { get; set; }
    }
    public class VendedorMsg
    {
        public string CodVendedor { get; set; }
        public string Vendedor { get; set; }
        public string Correo { get; set; }
    }
}
