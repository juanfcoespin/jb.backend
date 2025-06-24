using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbp.msg.sap
{
    public class FiltroPickingProdME
    {
        public string articulo;

        public string linea { get; set; }
        public string docNumOF { get; set; }
        public string CodBodegaMat { get; set; }
    }
    public class CantidadesReservadasPorLoteMsg
    {
        public int DocNumST { get; set; }
        public double Cantidad { get; set; }
        public int DocNumOF { get; set; }
        public string Lote { get; set; }
        public string BodegaDestino { get; set; }
        public int IdSolicitudTraslado { get; set; }
        public int LineNum { get; set; }
    }
    public class ST_OF_LiberadasMsg
    {
        public int NumST { get; set; }
        public int NumOF { get; set; }
        public string FechaInicioOf { get; set; }
        public string Articulo { get; set; }
        public decimal CantidadPlanificada { get; set; }
        public string UnidadMedida { get; set; }
        public List<ST_ComponentesMsg> Componentes { get; set; }
        public int Id { get; set; }
        public string BodegaDestino { get; set; }
        public string BodegaOrigen { get; set; }

        public ST_OF_LiberadasMsg() {
            this.Componentes = new List<ST_ComponentesMsg>();
        }

    }
    public class UbicacionLoteMsg
    {
        public string Ubicacion { get; set; }
        public decimal Cantidad { get; set; }
    }
    public class ST_ComponentesMsg
    {
        public string CodArticulo { get; set; }
        public string Articulo { get; set; }
        public decimal Cantidad { get; set; }
        public string UnidadMedida { get; set; }
        public string Lote { get; set; }
        public List<UbicacionLoteMsg> Ubicaciones { get; set; }
        public string BodegaDestino { get; set; }
        public int LineNum { get; set; }
        public string BodegaOrigen { get; set; }
        public decimal CantidadReservada { get; set; }
    }
    public class ST_ComponentesDetalleMsg: ST_ComponentesMsg
    {
        public string Lote { get; set; }
        public decimal CantidadLote { get; set; }
        public string Bodega { get; set; }
        public string Ubicacion { get; set; }
    }

    public class StMsg
    {
        public string BodegaDestino;

        public string BodegaOrigen { get; set; }
        public List<LineStMsg> Lines { get; set; }
        public int DocNumOF { get; set; }
        public string Comentarios { get; set; }

        public StMsg() { 
            this.Lines = new List<LineStMsg>();
        }
    }
    public class LineStMsg
    {
        public double Cantidad { get; set; }
        public string CodArticulo { get; set; }
        public string BodegaOrigen { get; set; }
        public string BodegaDestino { get; set; }
        public List<LoteStMsg> Lotes { get; set; }
        public LineStMsg()
        {
            this.Lotes = new List<LoteStMsg>();
        }

    }
    public class LoteStMsg
    {
        public string Lote { get; set; }
        public double Cantidad { get; set; }
    }



}
