using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbp.msg.sap
{
    public enum EDocBase { 
        OrdenFabricacion,
        SolicitudTransferencia
    }
    public class SalidaBodegaMsg
    {
        public List<SalidaBodegaLineaMsg> Lineas { get; set; }
        public int DocNum { get; set; }
        public EDocBase DocBaseType { get; set; }
        public int IdDocBase { get; set; }
        public int IdOF { get; set; }
        public string From { get; set; }
        public string To { get; set; }
    }

    public class SalidaBodegaLineaMsg
    {
        public string CodArticulo { get; set; }
        public double Cantidad { get; set; }
        public string CodBodega { get; set; }
        public int LineNum { get; set; }
        public string Lote { get; set; }
    }
    public class TsBodegaMsg {
        public string CodBodegaDesde { get; set; }
        public string CodBodegaHasta { get; set; }
        private List<TsBodegaLineaMsg> _Lineas=new List<TsBodegaLineaMsg>();
        public List<TsBodegaLineaMsg> Lineas { 
            get { return this._Lineas; }
            set { this._Lineas = value; }
        }

        public int Serie { get; set; }
    }
    
    public class TsBodegaLineaMsg: LineaMsg
    {
        private List<AsignacionLoteMsg> _Lotes=new List<AsignacionLoteMsg>();
        public List<AsignacionLoteMsg> Lotes { 
            get { return this._Lotes; }
            set { this._Lotes = value; }   
        }
    }
    public class SubNivelBodegaMsg
    {
        public int id;
        public string codigo;
        public string descripcion;
    }
    public class PedidosPorProveedorMsg {
        public string Error { get; set; }
        public List<PedidoMsg> Pedidos { get; set; }
        public PedidosPorProveedorMsg() {
            this.Pedidos = new List<PedidoMsg>();
        }
    }
    public class EMProveedorMsg
    {
        public string Error { get; set; }
        public List<EntradaMercanciaQRMsg> EntradasMercancia { get; set; }
        public EMProveedorMsg()
        {
            this.EntradasMercancia = new List<EntradaMercanciaQRMsg>();
        }
    }
    public class EntradaMercanciaQRMsg {
        public string DocNum { get; set; }
        public string Articulo { get; set; }
        public string CodArticulo { get; set; }
        public string Fabricante { get; set; }
        public string Lote { get; set; }
        public string FechaIngreso { get; set; }
        public string FechaFabricacion { get; set; }
        public string FechaVencimiento { get; set; }
        public string FechaRetest { get; set; }
        public decimal Cantidad { get; set; }
        public int Bultos { get; set; }
    }

    public class PedidoMsg {

        public List<PedidoLineaMsg> Lineas { get; set; }
        public string NumPedido { get; set; }
        public string FechaPedido { get; set; }

        public PedidoMsg() {
            this.Lineas = new List<PedidoLineaMsg>();
        }
    }
    public class PedidoLineaMsg
    {
        public int LineNum { get; set; }
        public string CodArticulo { get; set; }
        public string Articulo { get; set; }
        public decimal CantidadPedido { get; set; }
        public decimal CantidadEntregada { get; set; }
        public decimal CantidadPendiente { get; set; }
    }

}
