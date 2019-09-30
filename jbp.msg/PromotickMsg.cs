using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbp.msg
{
    public class ParametroAceleradoresMsg
    {
        public string CodigosProductos { get; set; }
        public int Año { get; set; }
        public string Meses { get; set; }
    }
    public class AceleradorMsg
    {
        public string NroDocumento { get; set; }
        public int puntos { get; set; }
    }
    public class FacturaPromotickMsg : MensajeSalidaMsg
    {
        public int id { get; set; }
        public string fechaFactura { get; set; }
        public string numFactura { get; set; }
        public string descripcion { get; set; }
        public string numDocumento { get; set; }
        public int montoFactura { get; set; }
        public int puntos { get; set; }
    }
    public class FacturasPtkMsg
    {
        public List<FacturaPromotickMsg> facturas { get; set; }
    }
    public class RespPtkWSFacturasMsg: RespPtkMsg
    {
        public string numFactura { get; set; }
    }
    public class RespPtkMsg {
        public int codigo { get; set; }
        public string mensaje { get; set; }
    }
    public class RespPtkAcelerador:RespPtkMsg {
        public string NroDocumento { get; set; }
    }
    public class RespuestasPtkWsFacturasMsg
    {
        public List<RespPtkWSFacturasMsg> respuesta { get; set; }
    }
}
