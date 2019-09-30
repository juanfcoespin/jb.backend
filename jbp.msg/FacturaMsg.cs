using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbp.msg
{
    public class OrdenMsg {
        public int IdOrden { get; set; }
        public string CodOrden { get; set; }
        public OrdenMsg() { }
    }
    public class FacturaServicioMsg:OrdenMsg {
        public int IdFactura { get; set; }
        public string CodFactura { get; set; }
        public string Sitio { get; set; }
        public string FechaFactura { get; set; }
        public string RazonSocial { get; set; }
        public string Ruc { get; set; }
        public string MontoBruto { get; set; }
        public string Descuento { get; set; }
        public string Impuestos { get; set; }
        public string Monto { get; set; }
        public string Moneda { get; set; }
        public string TerminoPago { get; set; }
        public string CiudadEmisionFactura { get; set; }
        public string FechaVencimiento { get; set; }
        public string Vendedor { get; set; }
        public string DireccionCliente { get; set; }
        public string CiudadCliente { get; set; }
        public string TelefonoCliente { get; set; }
        public string ConocidoComo { get; set; }
        public string ContactoCliente { get; set; }
        public string mailCliente { get; set; }
        public FacturaServicioMsg() { }
    }
    public class FacturaMsg: FacturaServicioMsg
    {
        public string NumGuia { get; set; }
        public FacturaMsg() { }
    }
    public class FacturaTrandinaMsg
    {
        public int Id { get; set; }
        public string CodigoCliente;
        public List<DetalleFacturaTrandinaMsg> Detalle;
        public string Documento { get; set; }
        public string FechaFactura { get; set; }
        public string Observaciones { get; set; }
        public string PuntoEntrega { get; set; }
        public int UsuarioRegistro { get; set; }

        public FacturaTrandinaMsg() {
            //TODO: Revisar que este es el código con el que hay en enviar el WS trandina
            this.UsuarioRegistro = 326;
        }
    }
    public class RegistroFacturaTercerosMsg {
        public int IdFactura { get; set; }
        public string TipoTercero { get; set; }
        public RegistroFacturaTercerosMsg() { }
    }
    
    
}
