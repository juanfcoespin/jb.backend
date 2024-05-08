using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using jbp.msg;

using jbp.business.hana;

namespace jbp.services.rest.Controllers
{
    
    public class ProductController : ApiController
    {
        [HttpGet]
        [Route("api/product/getSellProducts")]
        public List<ProductMsg> GetSellProducts()
        {
            return ProductBusiness.GetSellProducts();
        }
        [HttpGet]
        [Route("api/product/getStockPt/{codArticulo}")]
        public object GetStockPt(string codArticulo)
        {
            return ProductBusiness.GetStockPt(codArticulo);
        }

    }
}
