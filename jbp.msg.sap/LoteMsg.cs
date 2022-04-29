using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbp.msg.sap
{
    public class LoteMsg
    {
        public string lote { get; set; }
        public string fechaFab { get; set; }
        public string fechaVen { get; set; }
    }

    public class CantidadLoteMsg
    {
        public double Disponible { get; set; }
        public string Lote { get; set; }
        public string CodBodega { get; set; }
    }
}
