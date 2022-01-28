using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using jbp.msg.sap;
using jbp.business.hana;

namespace jbp.services.rest.Controllers
{
    public class FacturaController : ApiController
    {
   
        [HttpPost]
        [Route("api/factura/updateFolioFactExportacion")]
        public string updateFolioNumFactExportacion([FromBody]FactExportacionMe me)
        {
            return FacturaBusiness.updateFolioNumFactExportacion(me);
        }
    }
}
