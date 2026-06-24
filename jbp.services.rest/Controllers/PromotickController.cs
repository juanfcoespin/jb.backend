using jbp.business.hana;
using jbp.msg;
using jbp.msg.sap;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

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

        [HttpGet]
        [Route("api/promotick/getNotasDeCreditoEnviadas/{fecha}")]
        public HttpResponseMessage GetNCEnviadasByRuc(string fecha)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    mensaje = HttpStatusCode.OK,
                    datos = new ParticipantePtkBusiness().GetNCEnviadasByRuc(fecha)
                });
            }
            catch (Exception error)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    message = "Ocurrió un problema al obtener las notas de credito",
                    error = error.Message
                });
            }
        }


        // enviar notas de credito manuales
        [HttpPost]
        [Route("api/promotick/setNcManuales")]
        public List<RespPtkWSFacturasMsg> setNcManuales(List<DocumentoPromotickMsg> me)
        {
            me.ForEach(doc =>
            {
                doc._customDescription = true;
                if (doc.montoFactura > 0)
                    doc.montoFactura = -1 * doc.montoFactura;
                if (doc.puntos > 0)
                    doc.puntos = -1 * doc.puntos;
            });
            var ms = new SendDocWsPtk().SendDocumentosToPromotickWS(me);
            return ms.respuesta;
        }

        [HttpGet]
        [Route("api/promotick/getParticipantesPorActualizar")]
        public List<ParticipantesPuntosMsg> GetParticipantesPorActualizar()
        {
            var participantePtkBusiness = new ParticipantePtkBusiness();
            return participantePtkBusiness.GetParticipantesToUpdate();
        }

        [HttpPost]
        [Route("api/promotick/registrarParticipantePorRuc")]
        public HttpResponseMessage RegistrarParticipantePorRuc(PeticionRucMsg datos)
        {
            var ruc = datos.ruc;
            try
            {
                var participantePtkBusiness = new ParticipantePtkBusiness();
                participantePtkBusiness.RegistrarParticipantePorRuc(ruc);
                return Request.CreateResponse(HttpStatusCode.OK, "Registro exitoso");
                
            }
            catch (Exception error)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    message = "Ocurrió un problema al actualizar los participantes",
                    error = error.Message
                });
            }
        }

        [HttpPost]
        [Route("api/promotick/actualizarParticipantePorRuc")]
        public HttpResponseMessage ActualizarParticipantePorRuc(PeticionRucMsg datos)
        {
            var ruc = datos.ruc;
            try
            {
                var participantePtkBusiness = new ParticipantePtkBusiness();
                participantePtkBusiness.ActualizarParticipantePorRuc(ruc);
                return Request.CreateResponse(HttpStatusCode.OK, "Actualización exitosa");
            }
            catch (Exception error)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    message = "Ocurrió un problema al actualizar los participantes",
                    error = error.Message
                });
            }
        }

        [HttpGet]
        [Route("api/promotick/actualizacionMasivaParticipantes")]
        public HttpResponseMessage ActualizacionMasivaParticipantes()
        {
            try
            {
                var participantePtkBusiness = new ParticipantePtkBusiness();
                bool resp = participantePtkBusiness.ActualizacionMasivaParticipantes();
                if (resp)
                    return Request.CreateResponse(HttpStatusCode.OK, "Actualización exitosa");
                else
                    return Request.CreateResponse(HttpStatusCode.OK, "Error en la actualización del Participante");
            }
            catch (Exception error)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    message = "Ocurrió un problema al actualizar los participantes",
                    error = error.Message
                });
            }
        }

        [HttpGet]
        [Route("api/promotick/registroMasivoParticipantes")]
        public HttpResponseMessage RegistroMasivoParticipantes()
        {
            try
            {
                var participantePtkBusiness = new ParticipantePtkBusiness();
                bool resp = participantePtkBusiness.RegistroMasivoParticipantes();
                if (resp)
                    return Request.CreateResponse(HttpStatusCode.OK, "Registro exitoso");
                else
                    return Request.CreateResponse(HttpStatusCode.OK, "Error en el registro del Participante");
            }
            catch (Exception error)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    message = "Ocurrió un problema al registrar los participantes",
                    error = error.Message
                });
            }
        }

        /*
        public List<RespPtkWSFacturasMsg> setNcManuales(List<DocumentoPromotickMsg> me)
        {
            me.ForEach(doc => {
                doc._customDescription = true;
                if (doc.montoFactura > 0)
                    doc.montoFactura = -1 * doc.montoFactura;
                if (doc.puntos > 0)
                    doc.puntos = -1 * doc.puntos;
            });

            var ms = new List<RespPtkWSFacturasMsg>();
            ms.Add(new RespPtkWSFacturasMsg
            {
                codigo=1,
                mensaje="Proceso exitoso",
                numFactura= me[0].numFactura
            });
            ms.Add(new RespPtkWSFacturasMsg
            {
                codigo = -100,
                mensaje = "No existe participante con el número de documento enviado",
                numFactura = me[1].numFactura
            });
            ms.Add(new RespPtkWSFacturasMsg
            {
                codigo = -500,
                mensaje = "Ocurrió un error en el proceso",
                numFactura = me[2].numFactura
            });
            return ms;
        }
        */

    }
}
