using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using jbp.msg;

namespace jbp.utils
{
    public class SucursalUtils
    {
        public static SucursalMsg GetDatosSucursalByCodFactura(string me) {
            // El codigo de factura tiene la forma: D01-001-0005438, ND1-16-11-0015, 001-010-0053085
            var ms = new SucursalMsg();
            if (!string.IsNullOrEmpty(me))
            {
                var matrix = me.Split(new char[] {'-'});
                if (matrix.Length == 3)
                {
                    ms = new SucursalMsg
                    {
                        CodSucursal = matrix[0],
                        CodLinea = matrix[1]
                    };
                    ms.Direccion = "Pifo - Av. Interoceánica Km.23 1/2";
                    switch (ms.Establecimiento){
                        case "002;010;":
                            ms.Direccion = "GUAYAQUIL - C.C. La Gran Manzana Av. Francisco de Orellana, Km 4 1/2";
                            break;
                        case "003;001;":
                            ms.Direccion = "PUEMBO, Teran Barea S/N y Antonio Vallejo";
                            break;
                    }
                }
            }
            return ms;
        }
    }
}
