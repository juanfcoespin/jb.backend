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
    public class TransferenciaStockController : ApiController
    {
        [HttpPost]
        [Route("api/transferenciaStock")]
        public DocSapInsertadoMsg Transfer([FromBody]TsBodegaMsg me)
        {
            return TransferenciaStockBussiness.Transfer(me);
        }
    }
}