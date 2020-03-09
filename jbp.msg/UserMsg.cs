using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbp.msg
{
    public class LoginMsg
    {
        public string User{ get; set; }
        public string Pwd { get; set; }
    }
    public class RespAuthMsg
    {
        public string Nombre { get; set; }
        public List<string> Perfiles { get; set; }
        public string Error { get; set; }
        public RespAuthMsg() {
            this.Perfiles = new List<string>();
        }
    }
    public class UserMsg :LoginMsg{
        public string Nombre { get; set; }
        public List<string> Perfiles { get; set; }
    }

}
