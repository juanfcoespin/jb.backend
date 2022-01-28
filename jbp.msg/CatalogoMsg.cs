using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbp.msg
{
    public class CatalogoPesajeMsg
    {
        public List<ArticuloMsg> materiasPrimas { get; set; }
        public List<ArticuloMsg> productosTerminados { get; set; }
    }
}
