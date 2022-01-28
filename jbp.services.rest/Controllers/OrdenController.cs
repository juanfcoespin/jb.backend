using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using jbp.msg.sap;
using jbp.business.hana;
using System.Threading;


namespace jbp.services.rest.Controllers
{
    public class OrdenController : ApiController
    {
        
        public List<string> Post([FromBody]List<OrdenMsg> ordenes)
        {
            var ms = OrderBusiness.SaveOrders(ordenes);
            return ms;
        }

        [HttpGet]
        [Route("api/orden/getOrdersByVendor/{codVendor}")]
        public List<OrdenAppMsg> GetOrdersByVendor(int codVendor)
        {
            return OrderBusiness.GetOrdersByVendor(codVendor);
        }

    }
}