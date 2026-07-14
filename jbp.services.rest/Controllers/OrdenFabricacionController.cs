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
        public List<OrdenFabricacionLiberadaPesajeMsg> GetOfLiberadasPesaje(string codArticuloAFabricar, string codInsumo)
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
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, OrdenFabricacionBusiness.getOrdenFab(DocNum));
            }
            catch (Exception error)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    message = "Ocurrió un problema al obtener la OF",
                    error = error.Message
                });
            }
        }

        [HttpPost]
        [Route("api/of/campania/create")]
        public HttpResponseMessage crearCampania(CampaniaRequest datos)
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

        [HttpGet]
        [Route("api/of/campania/list")]
        public HttpResponseMessage listaCampanias()
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, OrdenFabricacionBusiness.listaCampanias());
            }
            catch (Exception error)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    message = "Ocurrió un problema al obtener la lista de campañas",
                    error = error.Message
                });
            }
        }

        [HttpGet]
        [Route("api/of/campania/{id}")]
        public HttpResponseMessage obtenerCampania(int id)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, OrdenFabricacionBusiness.obtenerCampania(id));
            }
            catch (Exception error)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    message = "Ocurrió un problema al obtener la campaña",
                    error = error.Message
                });
            }
        }

        [HttpDelete]
        [Route("api/of/campania/{id}")]
        public HttpResponseMessage eliminarCampania(int id)
        {
            try
            {
                OrdenFabricacionBusiness.eliminarCampania(id);
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "La campaña fue eliminada correctamente" });
            }
            catch (Exception error)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    message = "Ocurrió un problema al eliminar la campaña",
                    error = error.Message
                });
            }
        }

        [HttpDelete]
        [Route("api/of/campania/{nroOf}/{idCampania}")]
        public HttpResponseMessage eliminarOfDeCampania(int nroOf, int idCampania)
        {
            try
            {
                OrdenFabricacionBusiness.eliminarOfDeCampania(nroOf, idCampania);
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "La OF fue eliminada de la campaña correctamente" });
            }
            catch (Exception error)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    message = "Ocurrió un problema al eliminar la OF de la campaña",
                    error = error.Message
                });
            }
        }

        [HttpPut]
        [Route("api/of/campania/{id}")]
        public HttpResponseMessage actualizarCampania(int id, CampaniaUpdate datos){
            try{
                OrdenFabricacionBusiness.actualizarCampania(id, datos);
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "La campaña fue actualizada correctamente" });
            }
            catch (Exception error){
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new{
                    message = "Ocurrió un problema al actualizar la campaña",
                    error = error.Message
                });
            }
        }

        [HttpPost]
        [Route("api/of/campania/detalle/create")]
        public HttpResponseMessage crearDetalleCampania(List<OrdenFabricacion> datos){
            try{
                OrdenFabricacionBusiness.crearDetalleCampania(datos);
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "La OF fue agregada correctamente a la campaña" });
            }catch (Exception error){
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new{
                    message = "Ocurrió un problema al agregar la OF a la campaña",
                    error = error.Message
                });
            }
        }

        [HttpGet]
        [Route("api/of/campania/find/{filter}")]
        public HttpResponseMessage buscarCampania_OF(string filter){
            try{
                return Request.CreateResponse(HttpStatusCode.OK, OrdenFabricacionBusiness.buscarCampania_OF(filter));
            }catch (Exception error){
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new{
                    message = "Ocurrió un problema al buscar la campaña",
                    error = error.Message
                });
            }
        }

        [HttpGet]
        [Route("api/of/campania/find/{fechaInicio}/{fechaFin}")]
        public HttpResponseMessage buscarCampaniaPorFechas(string fechaInicio, string fechaFin){
            try{
                return Request.CreateResponse(HttpStatusCode.OK, OrdenFabricacionBusiness.buscarCampaniaPorFechas(fechaInicio, fechaFin));
            }catch (Exception error){
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new{
                    message = "Ocurrió un problema al buscar la campaña",
                    error = error.Message
                });
            }
        }



    }
}
