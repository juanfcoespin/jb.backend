using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbp.msg
{
    public class EntregaHojaRutaME
    {
        public string fechaDesde { get; set; }
        public string fechaHasta { get; set; }
        public string lugar { get; set; }

    }
    public class EntregaHojaRutaMS
    {
        public string Cliente { get; set; }
        public string NumFactura { get; set; }
        public string Fecha { get; set; }
        public string CantBultos { get; set; }
        public string Transporte { get; set; }
        public string Ciudad { get; set; }
        public string Observaciones { get; set; }
        public string NumeroGuia { get; set; }
        public string Bodega { get; set; }
        public string MargenSuperior { get; set; }
        public string MargenIzquierdo { get; set; }
        public string FechaImpresion { get; set; }
    }

    public class EntregaUrbanoME
    {
        public string fechaDesde { get; set; }
        public string fechaHasta { get; set; }
        public string bodega { get; set; }
    }
    public class EntregaUrbanoMS
    {
        public string DocNum { get; set; }
        public string Bodega { get; set; }
        public string Fecha { get; set; }
        public int CantBultos { get; set; }
        public string Cliente { get; set; }
        public string Cedula { get; set; }
        public string NumFactura { get; set; }
    }
}
