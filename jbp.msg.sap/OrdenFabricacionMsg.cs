using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbp.msg.sap
{
    public class ComponentesMsg: OFBaseMsg
    {
        public string UnidadMedida { get; set; }
        public decimal CantidadRequerida { get; set; }
        public bool RequiereRepesaje { get; set; }
        public List<CantidadLoteOFMsg> CantidadesPorLote { get; set; }
        public decimal CantidadPesada { get; set; }
        public int LineNumST { get; set; }
    }
    public class CantidadLoteOFMsg {
        public string Lote { get; set; }
        public decimal Cantidad { get; set; }
        public string FechaVence { get; set; }
        public string AnalisisMP { get; set; }
    }
    public class OFBaseMsg
    {
        public string CodigoArticulo { get; set; }
        public string Descripcion { get; set; }
        
    }
    public class OrdenFabricacionLiberadaPesajeMsg : OFBaseMsg
    {
        public int NumOrdenFabricacion { get; set; }
        public string Lote { get; set; }
    }
    public class OFMasComponentesMsg {
        public int IdOf { get; set; }
        public int IdST { get; set; }
        public int NumOrdenFabricacion { get; set; }
        public string CodArticulo { get; set; }
        public string Descripcion { get; set; }
        public string BodegaDesde { get; set; }
        public string BodegaHasta { get; set; }
        public string LotePT { get; set; }
        public List<ComponentesMsg> Componentes { get; set; }
        

        public OFMasComponentesMsg() {
            this.Componentes = new List<ComponentesMsg>();
        }
    }

    public class CampaniaUpdate{
        public string NombreCampania { get; set; }
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
    }

    public class CampaniaRequest{
        public string NombreCampania { get; set; }
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public List<OrdenFabricacion> OrdenesFabricacion { get; set; }
    }

    public class OrdenFabricacion{
        public int IdST { get; set; }
        public int Id { get; set; }
        public int DocNum { get; set; }
        public string CodArticulo { get; set; } = string.Empty;
        public string Articulo { get; set; } = string.Empty;
        public string Lote { get; set; } = string.Empty;
        public DateTime? FechaFabricacion { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public DateTime? FechaFinalizacion { get; set; }
        public DateTime? FechaCierre { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string campaniaId { get; set; } = string.Empty;
    }
}
