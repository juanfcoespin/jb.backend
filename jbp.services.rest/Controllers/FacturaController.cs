using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using jbp.msg;
using jbp.business;

namespace jbp.services.rest.Controllers
{
    public class FacturaController : ApiController
    {
        [HttpGet]
        [Route("api/factura/getListToSendTrandina")]
        public List<FacturaTrandinaMsg> GetFacturasToSendTrandina()
        {
            var ms = FacturaBusiness.GetFacturasToSendTrandina();
            return ms;
        }
        [HttpPut]
        [Route("api/factura/registrarFacturasEnvioTerceros")]
        public bool RegistrarFacturaEnvioTerceros([FromBody]List<RegistroFacturaTercerosMsg> me)
        {
            var ms = FacturaBusiness.RegistrarFacturaEnvioTerceros(me);
            return ms;
        }
        [HttpPut]
        [Route("api/factura/desregistrarFacturasEnvioTerceros")]
        public bool DesregistrarFacturaEnvioTerceros([FromBody] List<FacturaTrandinaMsg> me)
        {
            var ms = FacturaBusiness.DesregistrarFacturaEnvioTerceros(me);
            return ms;
        }
    }
}
