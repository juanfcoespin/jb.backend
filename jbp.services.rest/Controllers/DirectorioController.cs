using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using jbp.business.hana;
using jbp.msg;

namespace jbp.services.rest.Controllers
{
    public class DirectorioController : ApiController
    {
        [HttpGet]
        [Route("api/directorio")]
        public List<ContactoMsg> GetDirectorio()
        {
            return DirectorioBusiness.GetDirectorio();
        }
    }
}
