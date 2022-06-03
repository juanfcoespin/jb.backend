using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using jbp.msg;
using jbp.msg.sap;
using jbp.business.hana;

namespace jbp.services.rest.Controllers
{
    public class VendedorController : ApiController
    {
        [HttpGet]
        [Route("api/vendor/getCarteraByCodVendedor/{codVendedor}")]
        public List<CarteraMsg> GetCarteraByCodVendedor(string codVendedor)
        {
            return VendedorBusiness.GetCarteraByCodVendedor(codVendedor);
        }

        [HttpPost]
        [Route("api/vendor/login")]
        public RespAuthMsg Login([FromBody]LoginMsg me)
        {
            return UserBusiness.GetVendorUser(me);
        }
    }
}
