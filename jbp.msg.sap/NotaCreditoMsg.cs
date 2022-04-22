using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbp.msg.sap
{
    public class NotaCreditoPPMsg
    {
        public string CodCliente { get; set; }
        public string Comentario { get; set; }
        public eNcPPType TipoDescPP { get; set; }
        public int FolioNumFacturaRelacionada { get; set; }
        public double TotalNC { get; set; }
    }
    public enum eNcPPType
    {
        Veterinario,
        HumanaFarmaceutico_0,
        HumanaFarmaceutico_12
    }
}
