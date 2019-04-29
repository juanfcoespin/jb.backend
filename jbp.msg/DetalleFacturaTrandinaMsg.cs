using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbp.msg
{
    public class DetalleFacturaTrandinaMsg
    {
        public int Cantidad { get; set; }
        public string CodigoArticulo { get; set; }
        public decimal Subtotal { get; set; }

        public DetalleFacturaTrandinaMsg() { }
    }
}
