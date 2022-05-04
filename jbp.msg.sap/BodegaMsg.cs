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
    
    public class TsBodegaLineaMsg
    {
        public double Cantidad { get; set; }
        private List<LoteEscogidoMsg> _Lotes=new List<LoteEscogidoMsg>();
        public List<LoteEscogidoMsg> Lotes { 
            get { return this._Lotes; }
            set { this._Lotes = value; }   
        }
        public string CodArticulo { get; set; }
    }
    public class LoteEscogidoMsg
    {
        public string Lote { get; set; }
        public double Cantidad { get; set; }
    }
}
