using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Http;

using jbp.msg;
using jbp.business.hana;

namespace jbp.services.rest.Controllers
{
    public class EntregaController : ApiController
    {
        [HttpPost]
        [Route("api/entrega/urbano")]
        public List<EntregaUrbanoMS> Urbano([FromBody] EntregaUrbanoME me)
        {
            //2021-06-04T20:58:04.373Z
            var ms = EntregaBusiness.GetEntregasUrbano(me);
            return ms;
        }

        [HttpPost]
        [Route("api/entrega/hojaRuta")]
        public List<EntregaHojaRutaMS> HojaRuta([FromBody] EntregaHojaRutaME me)
        {
            //2021-06-04T20:58:04.373Z
            var ms = EntregaBusiness.GetEntregasHojaRuta(me);
            return ms;
        }
    }
}