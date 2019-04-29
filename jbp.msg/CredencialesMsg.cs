using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbp.msg
{
    public class CredencialesMsg
    {
        public string usr { get; set; }
        public string pwd { get; set; }
        public CredencialesMsg() { }
    }
    public class CredencialesTrandinaMsg : CredencialesMsg
    {
        public string CodRegistro { get; set; }
        public CredencialesTrandinaMsg() { }
    }
}
