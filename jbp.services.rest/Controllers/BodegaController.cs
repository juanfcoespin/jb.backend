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
    public class BodegaController : ApiController
    {

        [HttpGet]
        [Route("api/bodega/getSubnivelesAlmacen")]
        public List<SubNivelBodegaMsg> GetSubnivelesAlmacen()
        {
            return BodegaBusiness.GetSubnivelesAlmacen();
        }

    }
}