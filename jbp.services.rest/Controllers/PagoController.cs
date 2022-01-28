using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Http;
using jbp.business.hana;
using jbp.msg.sap;

namespace jbp.services.rest.Controllers
{
    public class PagoController : ApiController
    {
        public List<string> Post([FromBody] List<PagoMsg> pagos)
        {
            var ms = new PagoBusiness().SavePagos(pagos);
            return ms;
        }
    }
}