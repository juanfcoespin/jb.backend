using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTools.Rest;
using jbp.msg;
using TechTools.Core.Hana;

namespace jbp.business.hana
{
    public class ParticipantePtkBusiness: BaseWSPtk
    {
        public string RucParticipante;
        public void RegistrarParticipante(string ruc, string numFactura=null)
        {
            if (string.IsNullOrEmpty(ruc))
                return;
            var me = SocioNegocioBusiness.GetParticipantePuntosByRuc(ruc);
            me.email = SocioNegocioBusiness.GetUnSoloCorreo(me.email); //promotick no soporta mas de un correo
            RegistrarParticipante(me, numFactura);
        }
        public void RegistrarParticipante(ParticipantesPuntosMsg me, string numFactura = null) {
            if (me == null || string.IsNullOrEmpty(me.nroDocumento))
                return;
            this.RucParticipante = me.nroDocumento;
            me.estado = 1; //Para registrarlo en ptk
            var url = string.Format("{0}/{1}", conf.Default.ptkWsUrl, "gstparticipantes");
            var rc = new RestCall();
            var resp=(RespWsMsg)rc.SendPostOrPut(url, typeof(RespWsMsg), me, typeof(ParticipantesPuntosMsg), RestCall.eRestMethod.POST, this.credencialesWsPromotick);
            resp.mensaje = string.Format("NumFactura: {0} {1}", numFactura, resp.mensaje);
            GestionarRespuestaRegistrarParticipante(resp,me);
            new SocioNegocioBusiness().RegistrarParticipanteComoSincronizado(me.nroDocumento);
        }
        private void GestionarRespuestaRegistrarParticipante(RespWsMsg respWS, ParticipantesPuntosMsg me)
        {
            if (respWS.codigo != 1) //proceso exitoso
            {
                var titulo = "Error en el registro del Participante " + this.RucParticipante;
                var participante = string.Format(@"
                    estado: {0}<br /> nombres: {1} <br />
                    email: {2}<br /> tipoDocumento: {3}<br />
                    nroDocumento: {4}<br /> clave: {5}<br />
                    fechaNacimiento: {6}<br /> celular: {7}<br />
                    telefono: {8}<br /> tipoGenero: {9}<br />
                    idCatalogo: {10}<br /> tipoCatalogo: {11}<br />
                    vendedor: {12}<br /> metaAnual: {13}<br />
                ",
                me.estado, me.nombres,
                me.email, me.tipoDocumento,
                me.nroDocumento, me.clave,
                me.fechaNacimiento, me.celular,
                me.telefono, me.tipoGenero,
                me.idCatalogo, me.tipoCatalogo,
                me.vendedor, me.metaAnual
                );


                var msg = "<b>Respuesta Servidor:</b> " + respWS.mensaje;
                msg+= "<br /><br /><b>Participante:</b><br />" + participante;
                EnviarPorCorreo(titulo, msg);
            }
        }
        public string GetRucClienteFromLogByNumDocumento(string numDocumento)
        {
            var sql = string.Format(@"
                select  
                 RUC
                from
                 JBP_LOG_ENVIO_DOCUMENTOS_PTK
                where 
                 NRO_DOCUMENTO='{0}' 
                 and to_char(now(),'yyyy-mm-dd')=to_char(FECHA_TX,'yyyy-mm-dd')
            ", numDocumento);
            var bc = new BaseCore();
            var ms = bc.GetScalarByQuery(sql);
            return ms;
        }
        public void SincronizarDatosDeParticipantes()
        {
            var participantesPorSincronizar = new SocioNegocioBusiness().GetParticipantesPorSincronizar();
            var pb = new ParticipantePtkBusiness();
            foreach (var participante in participantesPorSincronizar) {
                pb.RegistrarParticipante(participante);
            }
        }
    }
}
