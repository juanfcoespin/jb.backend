using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using jbp.business.oracle9i;
using jbp.msg;

namespace jbp.services.rest.Controllers
{
    public class CuentaController : ApiController
    {
        [HttpGet]
        [Route("api/cuenta/test")]
        public IObservable<int> GetList()
        {
            return CuentaBusiness.SetMontosPorPeriodoAsync();
        }
    }
}
