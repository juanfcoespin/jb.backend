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
    public class DirectorioController : ApiController{
        private DirectorioBusiness directorioBusiness = new DirectorioBusiness();

        [HttpGet]
        [Route("api/directorio")]
        public List<ContactoMsg> GetDirectorio(){
            return directorioBusiness.GetDirectorio();
        }

        [HttpPost]
        [Route("api/directorio")]
        public HttpResponseMessage CrearContacto([FromBody] ContactoMsg contacto){
            try{
                directorioBusiness.CrearContacto(contacto);
                return Request.CreateResponse(HttpStatusCode.OK, "Contacto creado");
            }
            catch (Exception error){
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, error);
            }
        }

        [HttpPut]
        [Route("api/directorio/{id}")]
        public HttpResponseMessage ActualizarContacto(int id, [FromBody] ContactoMsg contacto){
            try{
                directorioBusiness.ActualizarContacto(id, contacto);
                return Request.CreateResponse(HttpStatusCode.OK, "Contacto actualizado");
            }
            catch (Exception error){
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, error);
            }
        }

        [HttpDelete]
        [Route("api/directorio/{id}")]
        public HttpResponseMessage EliminarContacto(int id){
            try{
                directorioBusiness.EliminarContacto(id);
                return Request.CreateResponse(HttpStatusCode.OK, "Contacto eliminado");
            }
            catch (Exception error){
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, error);
            }
        }
    }
}
