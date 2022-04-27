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
    }

    public class SalidaBodegaLineaMsg
    {
        public string CodArticulo { get; set; }
        public double Cantidad { get; set; }
        public string CodBodega { get; set; }
        public int LineNum { get; set; }
        public string Lote { get; set; }
    }
}
