using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using jbp.msg;

namespace jbp.services.rest.Controllers
{
    public class TestController : ApiController
    {
        [HttpGet]
        [Route("api/test")]
        public RespAuthMsg Get()
        {
            var perfiles = new List<string>();
            perfiles.Add("Ventas");
            return new RespAuthMsg
            {
                Nombre = "Juan Francisco Espín ",
                Perfiles = perfiles
            };
        }
    }
}
