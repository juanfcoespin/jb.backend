using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbp.msg.sap
{
    public class DashBoardsMsg
    {
        public string error { get; set; }
        public List<Dash> data { get; set; }
    }
    public class Dash
    {
        public string nombre { get; set; }
        public string url { get; set; }
        public List<ModulosMsg> modulos { get; set; }
        public string error { get; set; }
        public int id { get; set; }
        public string modulosStr { get; set; }
        public Dash() { 
            this.modulos = new List<ModulosMsg>();
        }
    }
}
