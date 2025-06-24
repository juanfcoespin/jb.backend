using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbp.msg.sap
{
    public class GastosAdicionalesMsg
    {
        public dynamic LineNum { get; set; }
        public dynamic ObjType { get; set; }
        public dynamic DocEntry { get; set; }
        
    }
    public class EntradaMercanciaMsg
    {
        public EntradaMercanciaMsg() {
            this.GastosAdicionales = new List<GastosAdicionalesMsg>();
        }
        public string CodProveedor { get; set; }
        public List<EntradaMercanciaLineaMsg> Lineas { get; set; }
        
        public int DocNumOrigen { get; set; }
        public int IdDocOrigen { get; set; }
        public string Error { get; set; }
        public string IdEM { get; set; }
        public int DocNumEntradaMercancia { get; set; }
        public string FechaPedido { get; set; }
        public string responsable { get; set; }
        public string Comentario { get; set; }
        public string tipo { get; set; }
        public List<GastosAdicionalesMsg> GastosAdicionales { get; set; }

        public static void SetCodBodegaEnLineas(List<EntradaMercanciaLineaMsg> lineas)
        {
            var codBodega=getCodBodega(lineas);
            lineas.ForEach(linea =>
            {
                linea.CodBodega = codBodega;
            });
        }
        public static string getCodBodega(List<EntradaMercanciaLineaMsg> lineas) {
            if (lineas.Count > 0 && !string.IsNullOrEmpty(lineas[0].CodArticulo))
            {
                try
                {
                    if (lineas[0].AsignacionesLote.Count > 0)
                    {
                        var ubicacion = lineas[0].AsignacionesLote[0].Ubicacion;
                        if (!string.IsNullOrEmpty(ubicacion))
                        {
                            var matrix = ubicacion.Split('-');
                            return matrix[0];
                        }
                    }
                }
                catch{}
            }
            throw new Exception($"No se ha podido establecer el codBodega del producto {lineas[0].CodArticulo}");
        }
    }
    
    public class EntradaMercanciaLineaMsg {
        public string id { get; set; }
        public string Articulo { get; set; }
        public string CodArticulo { get; set; }
        public string UnidadMedida { get; set; }
        public List<AsignacionLoteEMMsg> AsignacionesLote { get; set; }
        public string CodBodega { get; set; }
        public decimal CantidadPedido { get; set; }
        public decimal CantidadPendiente { get; set; }
        public int LineNum { get; set; }
    }
    public class AsignacionLoteEMMsg {
        public string id { get; set; }
        public string FechaFabricacion { get; set; } //yyyy-mm-dd
        public string FechaVencimiento { get; set; }
        public string FechaRetest { get; set; }
        public string LoteFabricante { get; set; }
        public string Fabricante { get; set; }
        public int Bultos { get; set; }
        public double Cantidad { get; set; }
        public string Lote { get; set; }
        public string Ubicacion { get; set; }
        public dynamic IdUbicacion { get; set; }
    }
}
