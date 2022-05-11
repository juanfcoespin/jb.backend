using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbp.msg.sap
{
    public class EntradaMercanciaMsg
    {
        public string CodProveedor { get; set; }
        public List<LineaMsg> Lineas { get; set; }
        public string CodBodega { get; set; }
    }
}
