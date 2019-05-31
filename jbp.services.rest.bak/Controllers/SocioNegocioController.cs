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
    public class SocioNegocioController : ApiController
    {
        [HttpGet]
        [Route("api/socioNegocio/getParticipanteByRuc/{ruc}")]
        public ParticipantesPuntosMsg GetParticipanteByRuc(string ruc)
        {
            return SocioNegocioBusiness.GetParticipanteByRuc(ruc);
        }
        [HttpGet]
        [Route("api/socioNegocio/getItemsByToken/{token}")]
        public List<SocioNegocioItemMsg> GetItemsBytoken(string token)
        {
            return SocioNegocioBusiness.GetItemsBytoken(token);
        }
        [HttpGet]
        [Route("api/socioNegocio/getVendedores")]
        public List<string> GetVendedores()
        {
            return SocioNegocioBusiness.GetVededores();
        }

        [HttpGet]
        [Route("api/socioNegocio/habilitadoParaCangearPuntos/{nroDocumento}")]
        public HabilitadoCanjearPuntosMS HabilitadoParaCangearPuntos(string nroDocumento)
        {
            return new HabilitadoCanjearPuntosMS { CodResp=1, Resp=false};
        }

        [HttpGet]
        [Route("api/socioNegocio/existeParticipante/{nroDocumento}")]
        public bool ExisteParticipante(string nroDocumento)
        {
            return SocioNegocioBusiness.ExisteParticipante(nroDocumento);
        }

        [HttpPost]
        [Route("api/socioNegocio/SaveParticipante")]
        public SavedMs SaveParticipante([FromBody]ParticipantesPuntosMsg me) {
            return SocioNegocioBusiness.SaveParticipante(me);
        }
        
    }
}