using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbp.msg.sap
{
    public class UbicacionesPorLoteMS
    {
        public string Error { get; set; }
        public string CodArticulo { get; set; }
        public string Articulo { get; set; }
        public string Lote { get; set; }
        public List<UbicacionCantidad> UbicacionesCantidad { get; set; }
        public UbicacionesPorLoteMS() { 
            this.UbicacionesCantidad = new List<UbicacionCantidad>();
        }
    }
    public class BodegasConUbicacionMS {
        public string Error { get; set; }
        public List<string> Bodegas { get; set; }
        public BodegasConUbicacionMS() { 
            this.Bodegas = new List<string>(); 
        }
    }
    public class UbicacionesMS
    {
        public string Error { get; set; }
        public List<string> Ubicaciones { get; set; }
        public UbicacionesMS()
        {
            this.Ubicaciones = new List<string>();
        }
    }
    public class UbicacionCantidad {
        public string Ubicacion { get; set; }
        public decimal Cantidad { get; set; }
    }
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
        public string Responsable { get; set; }
        public string CodArticulo { get; set; }
        public string Lote { get; set; }
        public string CodBodegaDesde { get; set; }
        public int IdUbicacionDesde { get; set; }
        public string UbicacionDesde { get; set; }
        public string CodBodegaHasta { get; set; }
        public double CantidadTotal { get; set; }
        public List<UbicacionCantidadMsg> UbicacionesCantidadHasta { get; set; }

        public TsBodegaMsg() {
            this.UbicacionesCantidadHasta = new List<UbicacionCantidadMsg>();
        }
    }
    public class UbicacionCantidadMsg
    {
        public string Ubicacion { get; set; }
        public double Cantidad { get; set; }
        public int IdUbicacion { get; set; }
    }
    public class TsBalanzasMsg
    {
        public string CodBodegaDesde { get; set; }
        public string CodBodegaHasta { get; set; }
        public List<TsBalanzasLineaMsg> Lineas { get; set; }
        public TsBalanzasMsg() {
            this.Lineas = new List<TsBalanzasLineaMsg>();
        }
    }

    public class TsBalanzasLineaMsg
    {
        public string CodArticulo { get; set; }
        public double Cantidad { get; set; }
        public string Lote { get; set; }
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
