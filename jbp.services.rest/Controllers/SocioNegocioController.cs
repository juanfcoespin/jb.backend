using System.Collections.Generic;
using System.Web.Http;

using jbp.msg;
using jbp.msg.sap;
using jbp.business.oracle9i;
using jbp.business.hana;

namespace jbp.services.rest.Controllers
{
    public class SocioNegocioController : ApiController
    {
        [HttpPost]
        [Route("api/socioNegocio/participanteCumplioMeta")]
        public object ParticipanteCumplioMeta([FromBody] ConsultaCumplimientoMetaParticipantesMe me)
        {
            return jbp.business.hana.SocioNegocioBusiness.ParticipanteCumplioMeta(me);
        }

        [HttpGet]
        [Route("api/socioNegocio/getCarteraByRucPrincipal/{rucPrincipal}")]
        public List<CarteraMsg> getCarteraByRucPrincipal(string rucPrincipal) { 
            return jbp.business.hana.SocioNegocioBusiness.GetCarteraByRucPrincipalCliente(rucPrincipal);
        }
        [HttpGet]
        [Route("api/socioNegocio/getClientes")]
        public List<object> getClientes()
        {
            return jbp.business.hana.SocioNegocioBusiness.GetClientes();
        }
        [HttpGet]
        [Route("api/socioNegocio/getVentasYPuntosMes/{rucPrincipal}")]
        public object getVentasYPuntosMes(string rucPrincipal)
        {
            return jbp.business.hana.SocioNegocioBusiness.getVentasYPuntosMesPorRucPrincipal(rucPrincipal);
        }
        [HttpGet]
        [Route("api/socioNegocio/getVentasYPuntosMes/")]
        public object getVentasYPuntosMes()
        {
            // para clientes que no participan en el plan puntos
            return null;
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

        

        [HttpPost]
        [Route("api/socioNegocio/SaveParticipante")]
        public SavedMs SaveParticipante([FromBody]ParticipantesPuntosMsg me) {
            return new jbp.business.oracle9i.SocioNegocioBusiness().SaveParticipante(me);
        }
        
        [HttpGet]
        [Route("api/socioNegocio/getProveedoresConPedidos")]
        public object GetProveedoresConPedidos()
        {
            return jbp.business.hana.SocioNegocioBusiness.GetProveedoresConPedidos();
        }
        [HttpGet]
        [Route("api/socioNegocio/getProveedoresConFacturasReserva")]
        public object GetProveedoresConFacturasReserva()
        {
            return jbp.business.hana.SocioNegocioBusiness.GetProveedoresConFacturasReserva();
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