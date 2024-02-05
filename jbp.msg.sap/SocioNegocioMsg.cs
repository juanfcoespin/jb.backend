using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbp.msg.sap
{
    public class ClientesToSendMailMs {
        public string Error { get; set; }
        public List<ClienteMsg> Clientes{ get; set; }
        public ClientesToSendMailMs() { 
            this.Clientes = new List<ClienteMsg>();
        }
    }
    public class ClienteMsg
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
    }
}
