using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbp.msg
{
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
