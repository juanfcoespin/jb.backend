using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbp.msg.sap
{
    public class LineaMsg
    {
        public double Cantidad { get; set; }
        public string CodArticulo { get; set; }
    }
    public class AsignacionLoteMsg
    {
        public string Lote { get; set; }
        public double Cantidad { get; set; }
        public List<UbicacionCantidadLoteMsg> UbicacionesCantidadDesde { get; set; }
        public AsignacionLoteMsg() {
            this.UbicacionesCantidadDesde = new List<UbicacionCantidadLoteMsg>();
        }
    }
    public class UbicacionCantidadLoteMsg
    {
        public string Ubicacion { get; set; }
        public double Cantidad { get; set; }
        public int IdUbicacion { get; set; }
    }
    public class DocSapInsertadoMsg
    {
        public string Id { get; set; }
        public int DocNum { get; set; }
        public string Error { get; set; }

    }

}
