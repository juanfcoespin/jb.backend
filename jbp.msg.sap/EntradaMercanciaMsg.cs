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
        
        public int NumPedido { get; set; }
        public int IdOrdenCompra { get; set; }
        public string Error { get; set; }
        public string IdEM { get; set; }
        public int DocNumEntradaMercancia { get; set; }
        public string FechaPedido { get; set; }
        public string responsable { get; set; }
    }
    
    public class EntradaMercanciaLineaMsg {
        public string id { get; set; }
        public string Articulo { get; set; }
        public string CodArticulo { get; set; }
        public List<AsignacionLoteEMMsg> AsignacionesLote { get; set; }
        public string CodBodega { get; set; }
        public decimal CantidadPedido { get; set; }
        public decimal CantidadPendiente { get; set; }
        public int LineNum { get; set; }
    }
    public class AsignacionLoteEMMsg : AsignacionLoteMsg {
        public string id { get; set; }
        public string FechaFabricacion { get; set; } //yyyy-mm-dd
        public string FechaVencimiento { get; set; }
        public string FechaRetest { get; set; }
        public string LoteFabricante { get; set; }
        public string Fabricante { get; set; }
        public int Bultos { get; set; }
    }
}
