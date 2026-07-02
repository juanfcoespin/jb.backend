using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using jbp.business.hana;
using jbp.msg.sap;
using jbp.msg;

namespace jbp.services.rest.Controllers
{
    public class OrdenFabricacionController : ApiController
    {
        //se quita esta api porque la cantidad pesada se inyecta en la trasferencia de stock
        /*[HttpPost]
        [Route("api/setCantPesadaComponenteOF")]
        public BoolMs TransferFromBalanzas([FromBody] CantPesadaComponenteOF me)
        {
            return BodegaBusiness.SetCantPesadaComponenteOF(me);
        }*/
        [HttpGet]
        [Route("api/of/getOfLiberadasPesaje")]
        public List<OrdenFabricacionLiberadaPesajeMsg> GetOfLiberadasPesaje()
        {
            return OrdenFabricacionBusiness.GetOfLiberadasPesaje();
        }
        [HttpGet]
        [Route("api/of/getOfLiberadasPesaje/{codArticuloAFabricar}")]
        public List<OrdenFabricacionLiberadaPesajeMsg> GetOfLiberadasPesaje(string codArticuloAFabricar)
        {
            return OrdenFabricacionBusiness.GetOfLiberadasPesaje(codArticuloAFabricar);
        }
        [HttpGet]
        [Route("api/of/getOfLiberadasPesaje/{codArticuloAFabricar}/{codInsumo}")]
        public List<OrdenFabricacionLiberadaPesajeMsg> GetOfLiberadasPesaje(string codArticuloAFabricar,  string codInsumo)
        {
            return OrdenFabricacionBusiness.GetOfLiberadasPesaje(codArticuloAFabricar, codInsumo);
        }

        [HttpGet]
        [Route("api/of/getComponentesOf/{docNum}")]
        public OFMasComponentesMsg GetComponentesOfByDocNum(int docNum)
        {
            return OrdenFabricacionBusiness.GetComponentesAPesarOfByDocNum(docNum);
        }
        
        [HttpGet]
        [Route("api/of/getComponentesOf/{docNum}/{codInsumo}")]
        public OFMasComponentesMsg GetComponentesOfByDocNum(int docNum, string codInsumo)
        {
            return OrdenFabricacionBusiness.GetComponentesAPesarOfByDocNum(docNum, codInsumo);
        }

        [HttpGet]
        [Route("api/of/getOrdenFab/{DocNum}")]
        public HttpResponseMessage getOrdenFab(int DocNum)
        {
            try{
                return Request.CreateResponse(HttpStatusCode.OK, OrdenFabricacionBusiness.getOrdenFab(DocNum));
            }
            catch (Exception error){
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    message = "Ocurrió un problema al obtener la OF",
                    error = error.Message
                });
            }
        }

        [HttpPost]
        [Route("api/of/campania/create")]
        public HttpResponseMessage crearCampania(CampaniaRequest datos){
            try{
                OrdenFabricacionBusiness.crearCampania(datos);
                return Request.CreateResponse(HttpStatusCode.OK, new{ message = "La campaña fue creada correctamente" });
            }   
            catch (Exception error){
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new{
                    message = "Ocurrió un problema al crear la campaña",
                    error = error.Message
                });
            }
        }

        [HttpGet]
        [Route("api/of/campania/list")]
        public HttpResponseMessage campaniaList(CampaniaRequest datos)
        {
            try
            {
                OrdenFabricacionBusiness.crearCampania(datos);
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "La campaña fue creada correctamente" });
            }
            catch (Exception error)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    message = "Ocurrió un problema al crear la campaña",
                    error = error.Message
                });
            }
        }

    }
}
