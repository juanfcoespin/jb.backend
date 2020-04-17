using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using jbp.msg;
using jbp.business.oracle9i;
using jbp.business.hana;

namespace jbp.services.rest.Controllers
{
    public class SocioNegocioController : ApiController
    {
        [HttpGet]
        [Route("api/socioNegocio/getByRuc/{ruc}")]
        public SocioNegocioMsg GetByRuc(string ruc)
        {
            return jbp.business.oracle9i.SocioNegocioBusiness.GetByRuc(ruc);
        }
        [HttpGet]
        [Route("api/socioNegocio/getParticipanteByRuc/{ruc}")]
        public ParticipantesPuntosMsg GetParticipanteByRuc(string ruc)
        {
            return jbp.business.oracle9i.SocioNegocioBusiness.GetParticipanteByRuc(ruc);
        }
        [HttpGet]
        [Route("api/socioNegocio/getParticipanteByRucFromERP/{ruc}")]
        public ParticipantesPuntosMsg GetParticipanteByRucFromERP(string ruc)
        {
            return jbp.business.oracle9i.SocioNegocioBusiness.GetParticipanteByRucFromERP(ruc);
        }
        [HttpGet]
        [Route("api/socioNegocio/getItemsByToken/{token}")]
        public List<SocioNegocioItemMsg> GetItemsBytoken(string token)
        {
            return jbp.business.oracle9i.SocioNegocioBusiness.GetItemsBytoken(token);
        }
        [HttpGet]
        [Route("api/socioNegocio/getVendedores")]
        public List<string> GetVendedores()
        {
            return jbp.business.oracle9i.SocioNegocioBusiness.GetVededores();
        }

        [HttpGet]
        [Route("api/socioNegocio/habilitadoParaCangearPuntos/{nroDocumento}")]
        public HabilitadoCanjearPuntosMS HabilitadoParaCangearPuntos(string nroDocumento)
        {
            return new HabilitadoCanjearPuntosMS { CodResp=1, Resp=true};
        }

        [HttpGet]
        [Route("api/socioNegocio/existeParticipante/{nroDocumento}")]
        public bool ExisteParticipante(string nroDocumento)
        {
            return jbp.business.oracle9i.SocioNegocioBusiness.ExisteParticipante(nroDocumento);
        }

        [HttpPost]
        [Route("api/socioNegocio/SaveParticipante")]
        public SavedMs SaveParticipante([FromBody]ParticipantesPuntosMsg me) {
            return new jbp.business.oracle9i.SocioNegocioBusiness().SaveParticipante(me);
        }
        
    }
}