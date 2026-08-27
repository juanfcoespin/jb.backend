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

        public RespuestasPtkWsFacturasMsg SendDocumentosToPromotickWS(List<DocumentoPromotickMsg> documentos)
        {
            if (documentos != null && documentos.Count == 0)
                return null;
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
                return null;
            InsertarDocumentosAEnviar(documentos);
            return SendDocumentosToWS(documentos);
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
                values(?, ?, ?, CURRENT_TIMESTAMP, ?, ?)
            ");
            new BaseCore().Execute(sql, new Dictionary<string, object> {
                {"@0",periodo },
                {"@1",acelerador.nroDocumento },
                {"@2",acelerador.puntos },
                {"@3",resp.codigo },
                {"@4",resp.mensaje }
            });
        }

        public RespuestasPtkWsFacturasMsg SendDocumentosToWS(List<DocumentoPromotickMsg> documentos, bool esNcAjuste=false){
            DocumentosPtkMsg me = new DocumentosPtkMsg { facturas = documentos };
            var url = string.Format("{0}/{1}", conf.Default.ptkWsUrl, "gsttransaccion");
            var reqMsg = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(me);
            DateTime dateReq = DateTime.Now;

            try{
                var rc = new RestCall();
                var resp = (RespuestasPtkWsFacturasMsg)rc.SendPostOrPut(url, typeof(RespuestasPtkWsFacturasMsg), me, typeof(DocumentosPtkMsg), RestCall.eRestMethod.POST, this.credencialesWsPromotick);

                DateTime dateRes = DateTime.Now;
                var resMsg = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(resp);
                InsertarLogWS(dateReq, dateRes, reqMsg, resMsg, url, "POST", 200);

                var msgHtml = new StringBuilder();
                msgHtml.Append("<h3>Resultados del envío a Promotick</h3>");
                msgHtml.Append("<table border='1' cellpadding='5' cellspacing='0'><tr><th>Num Documento</th><th>Monto Factura</th><th>Puntos</th><th>Num Factura</th><th>Estado</th><th>Mensaje</th></tr>");
                
                var failedJsons = new StringBuilder();
                bool todosExitosos = true;
                var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

                foreach (var reqDoc in documentos){
                    var resDoc = resp.respuesta != null ? resp.respuesta.FirstOrDefault(r => r.numFactura == reqDoc.numFactura) : null;
                    string estado = "Sin Respuesta";
                    string msj = "";
                    bool isFailed = false;
                    
                    if (resDoc != null)
                        if (resDoc.codigo == 1){
                            estado = "Exitoso";
                            msj = resDoc.mensaje;
                        }
                        else{
                            estado = "Fallido";
                            msj = resDoc.mensaje;
                            todosExitosos = false;
                            isFailed = true;
                        }
                    else{
                        todosExitosos = false;
                        isFailed = true;
                    }

                    if (isFailed){
                        var failedReq = new DocumentosPtkMsg { facturas = new List<DocumentoPromotickMsg> { reqDoc } };
                        string jsonReq = serializer.Serialize(failedReq);
                        failedJsons.AppendFormat("<p><strong>Factura: {0}</strong><br/><code>{1}</code></p>", reqDoc.numFactura, jsonReq);
                    }

                    msgHtml.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td></tr>",
                        reqDoc.numDocumento, reqDoc.montoFactura, reqDoc.puntos, reqDoc.numFactura, estado, msj);
                }
                msgHtml.Append("</table>");

                if (!todosExitosos && failedJsons.Length > 0){
                    msgHtml.Append("<h3>Documentos de las peticiones fallidas</h3>");
                    msgHtml.Append(failedJsons.ToString());
                }
                
                string tituloCorreo = todosExitosos ? "Envío exitoso a Promotick" : "Fallos en envío a Promotick";
                EnviarPorCorreo(tituloCorreo, msgHtml.ToString());

                if (!esNcAjuste)
                    ActualizarRespuestasWS(resp);
                else
                    RegistrarNCAjuste(resp, documentos);
                return resp;
            }
            catch (Exception e){
                DateTime dateRes = DateTime.Now;
                InsertarLogWS(dateReq, dateRes, reqMsg, e.Message, url, "POST", 500);

                e = ExceptionManager.GetDeepErrorMessage(e, ExceptionManager.eCapa.Business);
                EnviarPorCorreo(e.Message, "Jbp-Promotick");
                var ms = new List<RespPtkWSFacturasMsg>();
                ms.Add(new RespPtkWSFacturasMsg { 
                    mensaje=$"Error: {e.Message+e.StackTrace}"
                });
                return new RespuestasPtkWsFacturasMsg{
                    respuesta = ms
                };
            }
        }

        private void InsertarLogWS(DateTime dateReq, DateTime dateRes, string msgReq, string msgRes, string url, string metodo, int statusCode){
            try{
                var sql = @"
                    INSERT INTO JB_LOG_PROMOTICK (DATE_REQ, DATE_RES, MSG_REQ, MSG_RES, URL, METODO, STATUS_CODE) 
                    VALUES (?, ?, ?, ?, ?, ?, ?)";

                new BaseCore().Execute(sql, new Dictionary<string, object> {
                    {"@0", dateReq},
                    {"@1", dateRes},
                    {"@2", msgReq ?? ""},
                    {"@3", msgRes ?? ""},
                    {"@4", url},
                    {"@5", metodo},
                    {"@6", statusCode}
                });
            }
            catch (Exception ex){
                // Ignorar o notificar si el log falla, para no afectar el flujo principal
                EnviarPorCorreo($"Error guardando log en LOG_PROMOTICK: {ex.Message}", "Error en InsertarLogWS");
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
                            VALUES(?, ?, ?, ?, ?, ?)
                        ");
                        bc.Execute(sql, new Dictionary<string, object> {
                            {"@0",nc.fechaFactura },
                            {"@1", nc.numFactura },
                            {"@2",nc.numDocumento },
                            {"@3", nc.montoFactura },
                            {"@4",nc.puntos },
                            {"@5",nc._description }
                        });
                        // se borra NC de documentos temporales
                        sql = string.Format(@"
                            delete from JBP_TMP_DOCS_PTK
                            where NUMDOCUMENTO=?
                            and NUMFACTURA=?
                        ");
                        bc.Execute(sql, new Dictionary<string, object> {
                            {"@0", nc.numDocumento },
                            {"@1", nc.numFactura }
                        });
                    }
                }

            });
        }

        public void InsertarDocumentosAEnviar(List<DocumentoPromotickMsg> documentos)
        {
            if (documentos == null)
                return;
            //documentos = CorregirFechasAnterioresAlMesActual(documentos);
            documentos.ForEach(documento => {
                var idDocumento = documento.id;
                if (SeEnvioAntesDocumento(documento))
                {
                    ActualizarReintentosEnvio(documento);
                    if (documento.numIntentosTx == 3)
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
                            ?,?,?,
                            ?,?,?,
                            NOW(),1,?,
                            ?, ?
                        )
                    "
                    );
                    var bc = new BaseCore();
                    bc.Execute(sql, new Dictionary<string, object> {
                        {"@0", documento.id }, {"@1",documento.fechaFactura }, {"@2",documento.numFactura },
                        {"@3",documento.numDocumento }, {"@4",documento.montoFactura }, {"@5",documento.puntos },
                        {"@6",documento.tipoDocumento },
                        {"@7",documento.fechaDocumentoOriginal}, {"@8",documento.descripcion }
                    });
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
                 set COD_RESPUESTA_WS=?,
                 MSG_RESPUESTA_WS=?
                where 
                 NRO_DOCUMENTO=? 
            ");
            bc.Execute(sql, new Dictionary<string, object> {
                {"@0", item.codigo }, {"@1",item.mensaje }, {"@2",item.numFactura }
            });
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
                 ID_DOCUMENTO =?
                 and NRO_DOCUMENTO=?
                 and TIPO_DOCUMENTO=?    
                 and PUNTOS=?
                 and to_char(FECHA_TX,'yyyy-mm-dd')=to_char(current_date,'yyyy-mm-dd')
            ");
            var numreg = new BaseCore().GetIntScalarByQuery(sql, new Dictionary<string, object> {
             { "@0", documento.id},
             { "@1", documento.numFactura},
             { "@2", documento.tipoDocumento},
             { "@3", documento.puntos},
            });
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
            if (documento.id == 0)
                return;
            documento.numIntentosTx +=1;
            var sql = string.Format(@"
                update JBP_LOG_ENVIO_DOCUMENTOS_PTK 
                set NUM_INTENTOS_TX=?,
                FECHA_TX=NOW()
                where ID_DOCUMENTO=? and TIPO_DOCUMENTO=?
            ");
            new BaseCore().Execute(sql, new Dictionary<string, object> {
                {"@0",documento.numIntentosTx }, {"@1",documento.id }, {"@2",documento.tipoDocumento }
            });
        }
    }
}
