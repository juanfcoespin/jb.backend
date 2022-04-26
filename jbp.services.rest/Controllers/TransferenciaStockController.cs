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
        
        public string Post([FromBody]SalidaBodegaMsg me)
        {
            try
            {
                var ms = TransferenciaStockBussiness.Save(me);
                return ms;
            }
            catch (Exception e) {
                return e.Message;
            }
            
        }
        public string Get()
        {
            return "Hola";
        }

    }
}