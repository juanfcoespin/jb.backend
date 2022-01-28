using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Http;
using jbp.business.hana;
using jbp.msg.sap;

namespace jbp.services.rest.Controllers
{
    public class ConfController : ApiController
    {
        [HttpGet]
        [Route("api/conf/getNumHojaRuta")]
        public int GetNumHojaRuta()
        {
            return ConfBusiness.GetNumHojaRuta();
        }
    }
}