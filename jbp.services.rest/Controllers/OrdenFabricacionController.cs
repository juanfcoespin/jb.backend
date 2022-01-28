using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using jbp.business.hana;
using jbp.msg.sap;

namespace jbp.services.rest.Controllers
{
    public class OrdenFabricacionController : ApiController
    {
        [HttpGet]
        [Route("api/of/getOfLiberadas")]
        public List<string> GetOfLiberadas()
        {
            return OrdenFabricacionBusiness.GetOfLiberadas();
        }

        [HttpGet]
        [Route("api/of/getComponentesOf/{docNum}")]
        public List<OrdenFabricacionMsg> GetComponentesOfByDocNum(int docNum)
        {
            return OrdenFabricacionBusiness.GetComponentesOfByDocNum(docNum);
        }
    }
}
