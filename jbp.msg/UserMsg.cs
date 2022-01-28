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
        public List<string> GruposDirectorioActivo{ get; set; }
        public string Error { get; set; }
        public string correo { get; set; }

        public RespAuthMsg() {
            this.Perfiles = new List<string>();
            this.GruposDirectorioActivo = new List<string>();
        }

        public bool IsVendor()
        {
            if (this.Perfiles.Count> 0)
            {
                return this.Perfiles.Exists(p => p.ToLower().Equals("vendedor"));
            }
            return false;
        }
    }
    public class RespAuthVendorMsg: RespAuthMsg {
        public int IdVendor { get; set; }
    }
    public class UserMsg :LoginMsg{
        public string Nombre { get; set; }
        public List<string> Perfiles { get; set; }
    }
    public class ContactoMsg
    {
        public string CONTACTO;
        public string DEPARTAMENTO;
        public string PLANTA;
        public string Ext;
    }

}
