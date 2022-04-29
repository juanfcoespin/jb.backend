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
        public List<TsBodegaLineaMsg> Lineas { get; set; }
    }
    
    public class TsBodegaLineaMsg
    {
        public double Cantidad { get; set; }
        public List<LoteEscogidoMsg> Lotes { get; set; }
        public string CodArticulo { get; set; }
    }
    public class LoteEscogidoMsg
    {
        public string Lote { get; set; }
        public double Cantidad { get; set; }
    }
}
