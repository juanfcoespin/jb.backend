using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using jbp.msg;
using TechTools.Rest;
using TechTools.Exceptions;
using TechTools.Core.Hana;

namespace jbp.business.hana
{
    public class SendDocWsPtk: BaseWSPtk
    {

        public void SendDocumentosToPromotickWS(List<DocumentoPromotickMsg> documentos)
        {
            //valida si los datos del participante están correctos
            //previo a enviar a promotick
            var documentosAQuitar = new List<DocumentoPromotickMsg>();
            foreach(var d in documentos)
            {
                var errorParticipante = "";
                if (!new ParticipantePtkBusiness().ParticipanteValido(d.numDocumento, ref errorParticipante))
                {
                    var msg = string.Format(@"
                    No se envio la {0} porque hay un problema de validación con el participante:</br>
                        Num Documento: {1}</br>
                        Monto: {2}</br>
                        Ruc: {3}</br>
                        Fecha Documento: {4}</br></br>
                        Detalle de la validación:</br>{5}",
                        d.tipoDocumento, d.numFactura, d.montoFactura, d.numDocumento, d.fechaFactura, errorParticipante);
                    EnviarPorCorreo("Error en envio documento promotick", msg);
                    documentosAQuitar.Add(d);
                }
            }
            //solo se envian los documentos con participantes validados
            documentosAQuitar.ForEach(d =>documentos.Remove(d));
            if (documentos.Count == 0)
                return;
            InsertarDocumentosAEnviar(documentos);
            SendDocumentosToWS(documentos);
        }

        internal void SendAceleradoresToPromotickWS(List<AceleradoresMsg> aceleradores, string periodo)
        {
            var url = string.Format("{0}/{1}", conf.Default.ptkWsUrl, "gstacelerador");
            aceleradores.ForEach(acelerador =>
            {
                try
                {
                    var rc = new RestCall();
                    var resp = (RespPtkWsAceleradorMsg)rc.SendPostOrPut(url, typeof(RespPtkWsAceleradorMsg),
                        acelerador, typeof(AceleradoresMsg), RestCall.eRestMethod.POST, this.credencialesWsPromotick);
                    InsertLogEnvioAcelerador(resp, acelerador, periodo);
                }
                catch (Exception e)
                {
                    e = ExceptionManager.GetDeepErrorMessage(e, ExceptionManager.eCapa.Business);
                    EnviarPorCorreo(e.Message, "Jbp-Promotick");
                }
            });
        }

        private void InsertLogEnvioAcelerador(RespPtkWsAceleradorMsg resp, AceleradoresMsg acelerador, string periodo)
        {
            var sql = string.Format(@"
                insert into JBP_ACELERADORES(PERIODO, RUC, PUNTOS, FECHA_ENVIO, COD_RESPUESTA_WS, MSG_RESPUESTA_WS) 
                values('{0}', '{1}', {2}, CURRENT_TIMESTAMP, {3}, '{4}')
            ", periodo, acelerador.nroDocumento, acelerador.puntos, resp.codigo, resp.mensaje);
            new BaseCore().Execute(sql);
        }

        public void SendDocumentosToWS(List<DocumentoPromotickMsg> documentos, bool esNcAjuste=false)
        {
            DocumentosPtkMsg me = new DocumentosPtkMsg { facturas = documentos };
            try
            {
                var url = string.Format("{0}/{1}", conf.Default.ptkWsUrl, "gsttransaccion");
                var rc = new RestCall();
                var resp = (RespuestasPtkWsFacturasMsg)rc.SendPostOrPut(url, typeof(RespuestasPtkWsFacturasMsg),
                    me, typeof(DocumentosPtkMsg), RestCall.eRestMethod.POST, this.credencialesWsPromotick);
                if (!esNcAjuste)
                    ActualizarRespuestasWS(resp);
                else
                    RegistrarNCAjuste(resp, documentos);
            }
            catch (Exception e)
            {
                e = ExceptionManager.GetDeepErrorMessage(e, ExceptionManager.eCapa.Business);
                EnviarPorCorreo(e.Message, "Jbp-Promotick");
            }
        }

        private void RegistrarNCAjuste(RespuestasPtkWsFacturasMsg respuestasWS, List<DocumentoPromotickMsg> documentos)
        {
            respuestasWS.respuesta.ForEach(resp =>
            {
                if (resp.codigo == 1){//proceso exitoso
                    var nc = documentos.FirstOrDefault(d => d.numFactura == resp.numFactura);
                    if (nc != null) {
                        var bc = new BaseCore();
                        //se inserta la NC por ajuste
                        var sql = string.Format(@"
                            insert into JBP_NC_MANUALES(FECHA_FACTURA, NUM_FOLIO, RUC_PRINCIPAL, MONTO_FACTURA, PUNTOS, DESCRIPCION)
                            VALUES('{0}', '{1}', '{2}', {3}, {4}, '{5}')
                        ",nc.fechaFactura, nc.numFactura, nc.numDocumento, nc.montoFactura, nc.puntos, nc._description);
                        bc.Execute(sql);
                        // se borra NC de documentos temporales
                        sql = string.Format(@"
                            delete from JBP_TMP_DOCS_PTK
                            where NUMDOCUMENTO='{0}'
                            and NUMFACTURA='{1}'
                        ", nc.numDocumento, nc.numFactura);
                        bc.Execute(sql);
                    }
                }

            });
        }

        private void InsertarDocumentosAEnviar(List<DocumentoPromotickMsg> documentos)
        {
            if (documentos == null)
                return;
            //documentos = CorregirFechasAnterioresAlMesActual(documentos);
            documentos.ForEach(documento => {
                var idDocumento = documento.id;
                if (SeEnvioAntesDocumento(documento))
                {
                    ActualizarReintentosEnvio(documento);
                    if (documento.numIntentosTx > 3)
                        NotificarPorCorreoNumIntentosExedidos(documento);
                }
                else
                { //insertar en el log
                    var sql = string.Format(@"
                        insert into JBP_LOG_ENVIO_DOCUMENTOS_PTK(
                            ID_DOCUMENTO, FECHA_DOCUMENTO, NRO_DOCUMENTO,
                            RUC, MONTO, PUNTOS,
                            FECHA_TX, NUM_INTENTOS_TX, TIPO_DOCUMENTO, 
                            FECHA_DOCUMENTO_ORIGINAL, DESCRIPCION
                        )values(
                            {0},'{1}','{2}',
                            '{3}',{4},{5},
                            NOW(),1,'{6}',
                            '{7}', '{8}'
                        )
                    ", documento.id, documento.fechaFactura, documento.numFactura,
                    documento.numDocumento, documento.montoFactura, documento.puntos,
                    documento.tipoDocumento, 
                    documento.fechaDocumentoOriginal, documento.descripcion
                    );
                    var bc = new BaseCore();
                    bc.Execute(sql);
                }
            });
        }
        private void ActualizarRespuestasWS(RespuestasPtkWsFacturasMsg resp)
        {
            if (resp != null)
            {
                resp.respuesta.ForEach(item => {
                    if (item != null)
                    {
                        UpdateCodigoRespuestaFacturasWS(item);
                        GestionarRespuestaWS(item);
                    }
                });
            }
        }
        private void UpdateCodigoRespuestaFacturasWS(RespPtkWSFacturasMsg item)
        {
            // como en la respuesta no se identifica si es una nota de credito 
            // o factura, se filtra por la fecha de hoy
            // es muy poco probable que la fecha de la factura coicida con la fecha de la NC
            var bc = new BaseCore();
            var sql = string.Format(@"
                update JBP_LOG_ENVIO_DOCUMENTOS_PTK 
                 set COD_RESPUESTA_WS={0},
                 MSG_RESPUESTA_WS='{1}'
                where 
                 NRO_DOCUMENTO='{2}' 
            ", item.codigo, item.mensaje, item.numFactura);
            bc.Execute(sql);
        }
        private void GestionarRespuestaWS(RespPtkWSFacturasMsg resp)
        {
            if (resp.codigo == 1 || resp.codigo == -200) //1 exitoso, -200 el documento ya está registrado
                return;
            var rucCliente = new ParticipantePtkBusiness().GetRucClienteFromLogByNumDocumento(resp.numFactura);
            switch (resp.codigo)
            {
                case -100: //no esta registrado el participante en la bdd promotick
                    new ParticipantePtkBusiness().RegistrarParticipante(rucCliente, resp.numFactura);
                    break;
                case -150:
                    var msg = string.Format("El documento {0}, tiene una fecha con mes anterior al actual", resp.numFactura);
                    EnviarPorCorreo(msg, "Jbp-Promotick");
                    break;
            }
        }
        private List<DocumentoPromotickMsg> CorregirFechasAnterioresAlMesActual(List<DocumentoPromotickMsg> facturas)
        {
            for (int i = 0; i < facturas.Count; i++)
            {

                var fechaDocumento = facturas[i].fechaFactura;
                if (fechaDocumento != null)
                {
                    // extraigo el mes ej 25/02/2020
                    var mes = fechaDocumento.Split(new char[] { '/' })[1];
                    // si el mes es menor que el actual
                    // pongo la fecha actual en la factura
                    if (!string.IsNullOrEmpty(mes) && Convert.ToInt32(mes) < DateTime.Now.Month)
                    {
                        facturas[i].fechaDocumentoOriginal = fechaDocumento;
                        facturas[i].fechaFactura = DateTime.Now.ToString("dd/MM/yyyy");
                    }
                }
            }
            return facturas;
        }
        private bool SeEnvioAntesDocumento(DocumentoPromotickMsg documento)
        {
            var sql = string.Format(@"
                select count(*) from JBP_LOG_ENVIO_DOCUMENTOS_PTK
                where 
                 RUC ='{0}'
                 and NRO_DOCUMENTO='{1}'
                 and TIPO_DOCUMENTO='{2}'                
            ",
            documento.numDocumento, documento.numFactura, documento.tipoDocumento);
            var numreg = new BaseCore().GetIntScalarByQuery(sql);
            return numreg > 0;
        }
        private void NotificarPorCorreoNumIntentosExedidos(DocumentoPromotickMsg documento)
        {
            var msg = string.Format(@"
                Se ha procesado en envio al WS por {3} veces de promotick el documento {0} con numero {1}
            ",
            documento.tipoDocumento, documento.numFactura, documento.RespuestaWS, documento.numIntentosTx);
            EnviarPorCorreo("Jbp-Promotick",msg);
        }
        private void ActualizarReintentosEnvio(DocumentoPromotickMsg documento)
        {
            documento.numIntentosTx +=1;
            var sql = string.Format(@"
                update JBP_LOG_ENVIO_DOCUMENTOS_PTK 
                set NUM_INTENTOS_TX={0},
                FECHA_TX=NOW()
                where ID_DOCUMENTO={1} and TIPO_DOCUMENTO='{2}'
            ",
            documento.numIntentosTx, documento.id, documento.tipoDocumento);
            new BaseCore().Execute(sql);
        }
    }
}
