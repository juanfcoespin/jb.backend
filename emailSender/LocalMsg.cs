using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace emailSender
{
    public class EmailResp
    {
        public string Usuario { get; internal set; }
        public string Mail { get; internal set; }
        public string Error { get; internal set; }
        public string FilePath { get; internal set; }
    }
    public class UiMsg
    {
        public UiMsg() {
            this.CorreosEnviados = 0;
            this.CorreosNoEnviados = 0;
            this.ListaCorreosNoEnviados = new List<EmailResp>();
        }
        public int CorreosEnviados { get; set; }
        public int CorreosNoEnviados { get; set; }
        public List<EmailResp> ListaCorreosNoEnviados { get; set; }
        
    }
}
