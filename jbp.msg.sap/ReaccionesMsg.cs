using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbp.msg.sap
{
    public class ReaccionesMsg
    {
        public string fechaRegistro { get; set; }
        public string nombres { get; set; }
        public string apellidos { get; set; }
        public string sexo { get; set; }
        public string rangoEdad { get; set; }
        public int pesoKg { get; set; }
        public int alturaCm { get; set; }
        public string quienPadecioReaccion { get; set; }
        public string padeceOtraEnfermedad { get; set; }
    }
}
