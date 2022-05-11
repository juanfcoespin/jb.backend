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
    public class EntradaMercanciaController : ApiController
    {
        [HttpPost]
        [Route("api/EntradaMercancia")]
        public string EntradaMercancia([FromBody]EntradaMercanciaMsg me)
        {
            var ms = EntradaMercanciaBussiness.Ingresar(me);
            return ms;
        }
    }
}