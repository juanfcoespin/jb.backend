using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbp.msg
{
    public class ProductMsg
    {
        public string id;
        public string name;
        public List<PriceListMsg> prices;
        public decimal stock;
        public List<LoteMsg> lotes;
        
    }
    public class LoteMsg
    {
        public string lote;
        public string fechaFabricacion;
        public string fechaVencimiento;
        public int cantidad;

        public string codBodega { get; set; }
    }

    public class PriceListMsg
    {
        public string priceList;
        public decimal price;
        public string id;
    }
}
