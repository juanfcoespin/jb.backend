using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbp.msg.sap
{
    public class OrdenFabricacionMsg
    {
        public string Codigo { get; set; }
        public string UnidadMedida { get; set; }
        public string Descripcion { get; set; }
        public decimal Cantidad { get; set; }
    }
}
