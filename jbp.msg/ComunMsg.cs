using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace jbp.msg
{
    public class MensajeSalidaMsg {
        public string Error { get; set; }
    }
    public class RangoFechaMsg
    {
        public string Desde { get; set; }
        public string Hasta { get; set; }
        public RangoFechaMsg() { }
    }
    public class ListMS<T> : MensajeSalidaMsg {
        public List<T> List { get; set; }
    }
}
