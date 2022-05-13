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
        [Route("api/of/getOfLiberadasPesaje")]
        public List<OrdenFabricacionLiberadaPesajeMsg> GetOfLiberadasPesaje()
        {
            return OrdenFabricacionBusiness.GetOfLiberadasPesaje();
        }

        [HttpGet]
        [Route("api/of/getComponentesOf/{docNum}")]
        public OFMasComponentesMsg GetComponentesOfByDocNum(int docNum)
        {
            return OrdenFabricacionBusiness.GetComponentesAPesarOfByDocNum(docNum);
        }
    }
}
