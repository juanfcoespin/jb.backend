using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbp.msg
{
    public class MailMsg
    {
        public int IdCliente;
        public string Correo { get; set; }
        public string Mensaje { get; set; }
        public bool Enviado { get; set; }
        public string Titulo { get; set; }
    }
}
