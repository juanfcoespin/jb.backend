using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbp.msg.sap
{
    /*public class InfoDetalladaLoteMS
    {
        public decimal cantTotal;

        public string Error { get; set; }
        public string CodArticulo { get; set; }
        public string Articulo { get; set; }
        public string Lote { get; set; }
        public List<UbicacionCantidad> UbicacionesCantidad { get; set; }
        public string Estado { get; set; }
        public string CodPoe { get; set; }
        public string UnidadMedida { get; set; }
        public string LoteProveedor { get; set; }
        public string Fabricante { get; set; }
        public string FechaFabricacion { get; set; }
        public string FechaVencimiento { get; set; }
        public string FechaRetest { get; set; }
        public string Proveedor { get; set; }
        public string CondicionAlmacenamiento { get; set; }
        public string Bultos { get; set; }
        public string Observaciones { get; set; }

        public InfoDetalladaLoteMS() { 
            this.UbicacionesCantidad = new List<UbicacionCantidad>();
        }
    }*/
    public class msDetArticuloPorLote
    {
        public List<DetArticuloPorLote> Lotes { get; set; }
        public string Error { get; set; }

        public msDetArticuloPorLote() {
            this.Lotes = new List<DetArticuloPorLote>();
        }  
    }
    public class DetArticuloPorLote
    {
        public string Error { get; set; }
        public string Id { get; set; }
        public string CodArticulo { get; set; }
        public string Articulo { get; set; }
        public string Lote { get; set; }
        public string CodPoe { get; set; }
        public string Estado { get; set; }
        public string UnidadMedida { get; set; }
        public string LoteProveedor { get; set; }
        public string Fabricante { get; set; }
        public string FechaIngreso { get; set; }
        public string FechaFabricacion { get; set; }
        public string FechaVencimiento { get; set; }
        public string FechaRetest { get; set; }
        public string Proveedor { get; set; }
        public string CondicionAlmacenamiento { get; set; }
        public string Bultos { get; set; }
        public string Observaciones { get; set; }
        public string CondicionAlmacenamientoPT { get; set; }
        public string ResponsableEmpaque { get; set; }
        public string Cliente { get; set; }
        public decimal Cantidad { get; set; }
        public string CodPoePT { get; set; }
        public List<object> UbicacionesCantidad { get; set; }
        public string LoteFabricante { get; set; }
        public int CantidadPT { get; set; }
        public bool EsPT { get; set; }
        public string BodegaDestino { get; set; }
    }
    public class ContenidoUbicacionMS
    {
        public List<ContenidoUbicacionItemMS> Items { get; set; }
        public string Error { get; set; }
        public ContenidoUbicacionMS()
        {
            this.Items = new List<ContenidoUbicacionItemMS>();
        }
    }
    public class LotesCuarMS
    {
        public string CodArticulo { get; set; }
        public string Lote { get; set; }
        public string Estado { get; set; }
        public string CodBodega { get; set; }
        public string Cantidad { get; set; }
    }
    public class ContenidoUbicacionItemMS
    {
        public string Lote { get; set; }
        public string CodBodega { get; set; }
        public string CodArticulo { get; set; }
        public decimal Cantidad { get; set; }
        public string UnidadMedida { get; set; }
        public string Articulo { get; set; }
    }
    public class BodegasMS {
        public string Error { get; set; }
        public List<string> Bodegas { get; set; }
        public BodegasMS() { 
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
        public string Responsable { get; set; }
    }
    public enum eEstadoLote { 
        Liberado=0,
        AccesoDenegado=1,
        Bloqueado=2
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
        
        public string CodArticulo { get; set; }
        public string Lote { get; set; }
        public string Responsable { get; set; }
        
        public List<MovimientoTsMsg> movimientos { get; set; }

        public TsBodegaMsg() {
            this.movimientos = new List<MovimientoTsMsg>();
        }
    }
    public class MovimientoTsMsg {
        public double Cantidad { get; set; }
        public string CodBodegaDesde { get; set; }
        public string UbicacionDesde { get; set; }
        public string CodBodegaHasta { get; set; }
        public string UbicacionHasta { get; set; }
        public int IdUbicacionDesde { get; set; }
        public int IdUbicacionHasta { get; set; }
    }    
    public class TsFromPickingME
    {
        public string ClientId;

        public string Responsable { get; set; }
        public List<ComponenteMsg> Componentes { get; set; }
        public int Id { get; set; }
        public string BodegaDestino { get; set; }
        public string BodegaOrigen { get; set; }
        public int NumST { get; set; }
        public string NumOF { get; set; }
        public string BodegaProd { get; set; }
        public string Comentario { get; set; }

        public TsFromPickingME() { 
            this.Componentes = new List<ComponenteMsg>();
        }
    }
    public class ComponenteMsg
    {
        public string BodegaOrigen { get; set; }
        public string BodegaDestino { get; set; }
        public double CantidadEnviada { get; set; }
        public string CodArticulo { get; set; }
        public int LineNum { get; set; }
        public List<LoteComponenteMsg> Lotes { get; set; }
        public double CantidadRequerida { get; set; }
        public ComponenteMsg(){
            this.Lotes=new List<LoteComponenteMsg>();
        }
    }
    public class LoteComponenteMsg {
        public double CantidadEnviada { get; set; }
        public double CantidadReservada { get; set; }
        public string Lote { get; set; }
        public List<UbicacionCantidadMsg> Ubicaciones { get; set; }
        public LoteComponenteMsg() {
            this.Ubicaciones = new List<UbicacionCantidadMsg>();
        }

    }
    public class UbicacionCantidadMsg
    {
        public string Ubicacion { get; set; }
        public double Cantidad { get; set; }
        public int IdUbicacion { get; set; }
    }
    public class CamposOF
    {
        public string Estado { get; set; }
        public double CantidadPlanificada { get; set; }
        public string UnidadMedida { get; set; }
    }
    public class CantPesadaComponenteOF
    {
        public double CantPesada { get; set; }
        public int IdOf { get; set; }
        public string CodArticulo { get; set; }
    }
    public class TsBalanzasMsg
    {
        public string CodBodegaDesde { get; set; }
        public string CodBodegaHasta { get; set; }
        public int DocNumOF { get; set; }
        public List<TsBalanzasLineaMsg> Lineas { get; set; }
        public int IdST { get; set; }

        public TsBalanzasMsg() {
            this.Lineas = new List<TsBalanzasLineaMsg>();
        }
    }

    public class TsBalanzasLineaMsg
    {
        public string CodArticulo { get; set; }
        public List<TsBalanzasLoteMsg> Lotes { get; set; }
        public int LineNumST { get; set; }
        public int IdSt { get; set; }

        public TsBalanzasLineaMsg() {
            this.Lotes = new List<TsBalanzasLoteMsg>();
        }
    }
    public class TsBalanzasLoteMsg
    {
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
        public string UnidadMedida { get; set; }
        public string LoteFabricante { get; set; }
        public string CodPoe { get; set; }
    }

    public class PedidoMsg {

        public List<PedidoLineaMsg> Lineas { get; set; }
        public string DocNumOrigen { get; set; }
        public string Fecha { get; set; }
        public string IdDocOrigen { get; set; }
        public string CodProveedor { get; set; }

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
        public string UnidadMedida { get; set; }
    }

}
