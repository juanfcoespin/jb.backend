using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbp.msg
{
    public class CuentaMsg
    {
        public string Nivel1 { get; set; }
        public string Nivel2 { get; set; }
        public string Nivel3 { get; set; }
        public string Nivel4 { get; set; }
        public string Nivel5 { get; set; }
        public string Cuenta { get; set; }
        public string CuentaNombre { get; set; }
    }
    public class PlanCuentasProcesadoMsg: CuentaMsg {
        public double Real { get; set; }
        public double Presupuesto { get; set; }
        public PeriodoMsg Periodo { get; set; }
    }
}
