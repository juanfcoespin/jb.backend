using System.Collections.Generic;
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
        [Route("api/socioNegocio/getHistoricoClientesByNombre/{nombre}")]
        public SocioNegocioItemMS GetHistoricoClientesByNombre(string nombre)
        {
            return jbp.business.oracle9i.SocioNegocioBusiness.GetHistoricoClientesByNombre(nombre);
        }

        [HttpGet]
        [Route("api/socioNegocio/getByCodVendedor/{codVendedor}")]
        public List<ClientMsg> GetByCodVendedor(string codVendedor)
        {
            return jbp.business.hana.SocioNegocioBusiness.GetByCodVendedor(codVendedor);
        }
        [HttpGet]
        [Route("api/socioNegocio/getParticipanteByRuc/{ruc}")]
        public ParticipantesPuntosMsg GetParticipanteByRuc(string ruc)
        {
            //return jbp.business.oracle9i.SocioNegocioBusiness.GetParticipanteByRuc(ruc);
            return jbp.business.hana.ParticipantePtkBusiness.GetParticipantePuntosConDocumentosByRucPrincipal(ruc);
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
            return jbp.business.hana.SocioNegocioBusiness.GetItemsBytoken(token);
        }
        [HttpGet]
        [Route("api/socioNegocio/getVendedores")]
        public List<ItemCombo> GetVendedores()
        {
            return jbp.business.hana.VendedorBusiness.GetList();
        }

        [HttpGet]
        [Route("api/socioNegocio/habilitadoParaCangearPuntos/{nroDocumento}")]
        public HabilitadoCanjearPuntosMS HabilitadoParaCangearPuntos(string nroDocumento)
        {
            return new HabilitadoCanjearPuntosMS { CodResp=1, Resp=true};
            //return jbp.business.hana.SocioNegocioBusiness.HabilitadoParaCangearPuntos(nroDocumento);
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
        
        [HttpGet]
        [Route("api/socioNegocio/getProveedoresConPedidos")]
        public List<SocioNegocioItemMsg> GetProveedores()
        {
            return jbp.business.hana.SocioNegocioBusiness.GetProveedores();
        }

        [HttpGet]
        [Route("api/socioNegocio/getProveedoresEM")]
        public List<SocioNegocioItemMsg> GetProveedoresEM()
        {
            // trae los proveedores que tengas entradas de mercancía para 
            return jbp.business.hana.SocioNegocioBusiness.GetProveedoresEM();
        }

    }
}