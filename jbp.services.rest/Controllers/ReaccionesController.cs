using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Http;
using jbp.business.hana;
using jbp.msg.sap;

namespace jbp.services.rest.Controllers
{
    public class ReaccionesController : ApiController
    {
        public List<string> Post([FromBody] List<ReaccionesMsg> reacciones)
        {
            var ms = new ReaccionesBusiness().Save(reacciones);
            return ms;
        }
        public List<ReaccionesMsg> get()
        {
            var ms = new ReaccionesBusiness().GetReacciones();
            return ms;
        }

        [HttpGet]
        [Route("api/reacciones/getCatalogos")]
        public CatalogosReacciones getCatalogos() {
            var ms = new ReaccionesBusiness().GetCatalogos();
            return ms;
        }
    }
}