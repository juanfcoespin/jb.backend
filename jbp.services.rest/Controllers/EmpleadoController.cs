using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Http;

using jbp.msg;
using jbp.business.hana;

namespace jbp.services.rest.Controllers
{
    public class EmpleadoController : ApiController
    {
        [HttpGet]
        [Route("api/emplado/getCumple")]
        public object getCumple()
        {
            return jbp.business.hana.EmpleadoBusiness.getCumple();
        }
    }
}