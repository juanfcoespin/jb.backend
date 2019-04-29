using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbp.msg
{
    public class SucursalMsg
    {
        public string Direccion { get; set; }
        public string CodSucursal { get; set; }
        public string CodLinea { get; set; }
        public string Establecimiento {
            get {
                return string.Format("{0};{1};", CodSucursal, CodLinea);
            }
        }
    }
}
