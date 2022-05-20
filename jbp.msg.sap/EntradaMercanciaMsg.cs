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
        public List<EntradaMercanciaLineaMsg> Lineas { get; set; }
        public string CodBodega { get; set; }
    }
    public class EntradaMercanciaLineaMsg {
        public string CodArticulo { get; set; }
        public List<AsignacionLoteEMMsg> AsignacionesLote { get; set; }
    }
    public class AsignacionLoteEMMsg : AsignacionLoteMsg {
        public string FechaFabricacion { get; set; } //yyyy-mm-dd
        public string FechaVencimiento { get; set; }
        public string FechaRetest { get; set; }
        public string LoteFabricante { get; set; }
    }
}
