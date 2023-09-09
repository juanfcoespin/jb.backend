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
        public string AppName { get; set; }
    }
    public class ModulosAccesoMS {
        public bool Ventas { get; set; }
        public bool Bodega { get; set; }
        public bool Produccion { get; set; }
        public bool FarmacoVigilancia { get; set; }
        public bool Dashboards { get; set; }
        public bool ControlCalidad { get; set; }
    }
    public class ModulosMsg
    {
        public string Name { get; set; }
        public bool Checked { get; set; }
        
    }
    public class LogMsg
    {
        public string UserName { get; set; }
        public string AppName { get; set; }
        public string Obs { get; set; }
    }
    public class RespAuthMsg
    {
        public string Nombre { get; set; }
        //public List<string> Perfiles { get; set; }
        public List<string> GruposDirectorioActivo{ get; set; }
        public string Error { get; set; }
        public string correo { get; set; }
        public int IdUserSap { get; set; }
        public int IdVendor { get; set; }
        public string UserName { get; set; }
        public ModulosAccesoMS ModulosAcceso { get; set; }

        public RespAuthMsg() {
            this.GruposDirectorioActivo = new List<string>();
        }
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
