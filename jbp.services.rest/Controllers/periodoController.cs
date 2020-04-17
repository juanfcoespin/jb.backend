using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using jbp.msg;
using jbp.business.oracle9i;

namespace jbp.services.rest.Controllers
{
    public class periodoController : ApiController
    {
        [HttpGet]
        [Route("api/periodo/getList")]
        public ListMS<ItemCombo> GetList()
        {
            return PeriodoBusiness.GetList();
        }
    }
}
