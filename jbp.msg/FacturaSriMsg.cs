using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbp.msg
{
    public class facturaJBP:factura {
        private string _CodFactura;
        public string CodFactura {
            set {
                this._CodFactura = value;
                SetInfoTributaria(value);
            }
        }
        private void SetInfoTributaria(string codFactura)
        {

            //Ej. Codigos de factura: 001-010-0052662; 002-010-0011728
            var matrixCodFactura = codFactura.Split(new char[] {'-'});
            this.infoTributaria.razonSocial = "JAMES BROWN PHARMA C.A.";
            this.infoTributaria.nombreComercial = "JAMES BROWN PHARMA C.A.";
            this.infoTributaria.ruc = "1790462854001";
            this.infoTributaria.codDoc = "01"; //factura
            this.infoTributaria.estab = "";
        }

        public facturaJBP() { }
    }
    public class factura
    {
        public string id { get; set; }
        public string version { get; set; }
        public infoTributaria infoTributaria { get; set; }
        public infoFactura infoFactura { get; set; }
        public List<detalle> detalles { get; set; }
        public List<infoAdicional> infoAdicional { get; set; }
        public factura() {
            this.id = "comprobante";
            this.version = "2.1.0";
            infoTributaria = new infoTributaria();
            infoFactura = new infoFactura();
            detalles = new List<detalle>();
            infoAdicional = new List<infoAdicional>();
        }
    }
    public class infoAdicional
    {
        public string campoAdicional { get; set; }
        public string valor { get; set; }
        public infoAdicional() { }
    }
    public class infoTributaria
    {
        public string razonSocial { get; set; }
        public string nombreComercial { get; set; }
        public string ruc { get; set; }
        public string codDoc { get; set; }
        public string estab { get; set; }
        public string ptoEmi { get; set; }
        public string secuencial { get; set; }
        public string dirMatriz { get; set; }

        public infoTributaria() {
            this.razonSocial = "JAMES BROWN PHARMA C.A.";
            this.nombreComercial = "JAMES BROWN PHARMA C.A.";
            this.ruc = "1790462854001";
            this.codDoc = "";
        }
    }
    public class infoFactura {
        public string fechaEmision { get; set; }
        public string dirEstablecimiento { get; set; }
        public string tipoIdentificacionComprador { get; set; }
        public string razonSocialComprador { get; set; }
        public string identificacionComprador { get; set; }
        public decimal totalSinImpuestos { get; set; }
        public decimal totalDescuento { get; set; }
        public List<impuesto> totalConImpuestos { get; set; }
        public decimal propina { get; set; }
        public decimal importeTotal { get; set; }
        public string moneda { get; set; }
        public List<pago> pagos { get; set; }
        public infoFactura() {
            this.propina = 0;
            this.moneda = "Dolar";
            this.totalConImpuestos = new List<impuesto>();
            this.pagos = new List<pago>();
        }
    }
    public class detalle {
        public string codigoPrincipal { get; set; }
        public string descripcion { get; set; }
        public int cantidad { get; set; }
        public decimal precioUnitario { get; set; }
        public decimal descuento { get; set; }
        public decimal precioTotalSinImpuesto { get; set; }
        public List<impuesto> impuestos { get; set; }
        public detalle() { }
    }
    public class pago {
        public string formaPago { get; set; }
        public decimal total { get; set; }
        public int plazo { get; set; }
        public string unidadTiempo { get; set; }
        public pago() {
            this.unidadTiempo = "dias";
        }
    }
    public class impuesto
    {
        public int codigo { get; set; }
        public int codigoPorcentaje { get; set; }
        public int tarifa { get; set; }
        public decimal baseImponible { get; set; }
        public decimal valor { get; set; }

        public impuesto() { }
    }
}
