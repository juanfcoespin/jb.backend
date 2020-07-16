using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using jbp.business.hana;
using jbp.msg;

namespace jbp.services.rest.Controllers
{
    public class PromotickController : ApiController
    {
        [HttpGet]
        [Route("api/promotick/getEstadoCuenta/{ruc}")]
        public object GetEstadoCuenta(string ruc)
        {
            return new ParticipantePtkBusiness().GetEstadoCuentaByRuc(ruc);
        }
        [HttpGet]
        [Route("api/promotick/getDocumentosEnviados/{ruc}")]
        public List<DocumentoEnviadoMsg> GetDocumentosEnviados(string ruc)
        {
            return new ParticipantePtkBusiness().GetDocumentosEnviadosByRuc(ruc);
        }
    }
}
