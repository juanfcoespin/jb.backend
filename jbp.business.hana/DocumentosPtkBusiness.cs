using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TechTools.Exceptions;
using jbp.msg;
using System.Data;
using TechTools.Core.Hana;

namespace jbp.business.hana
{
    public class DocumentosPtkBusiness:BaseBusiness { 
        public void EnviarDocumentosAPromotick() {
            try
            {
                //var ppb = new ParticipantePtkBusiness();
                //ppb.SetRucInRucPrincipalNull();
                //ppb.SincronizarDatosDeParticipantes();

                ////para que se envíen los ajustes hay que insertar 
                ////manualmente los documentos
                //EnviarAjustes();

                //if (conf.Default.ptkEnviarFacturas)
                //    EnviarFacturas();
                if (conf.Default.ptkEnviarNC)
                    EnviarNotasCredito();
            }
            catch (Exception e)
            {
                e = ExceptionManager.GetDeepErrorMessage(e, ExceptionManager.eCapa.Business);
                EnviarPorCorreo("Jbp-Promotick",e.Message);
            }
        }

        /// <summary>
        /// cuando hay un desfase de la bdd de participantes
        /// y se envia mas o menos puntos de los que se deberia
        /// </summary>
        private void EnviarAjustes()
        {
            var documentos = GetDocumentosAjusteToSendPromotick();
            if (documentos == null || documentos.Count==0)
                return;
            new SendDocWsPtk().SendDocumentosToPromotickWS(documentos);
            SetDocumentosAjusteComoEnviado(documentos);
        }

        private void SetDocumentosAjusteComoEnviado(List<DocumentoPromotickMsg> documentos)
        {
            documentos.ForEach(documento => {
                var sql = string.Format(@"
                 UPDATE JBP_DOCUMENTOS_AJUSTE_PTK
                  SET ENVIADO=true
                 WHERE ID={0} AND NRO_DOCUMENTO='{1}'
                ", documento.id, documento.numFactura);
                new BaseCore().Execute(sql);
            });
        }

        private List<DocumentoPromotickMsg> GetDocumentosAjusteToSendPromotick()
        {
            try
            {
                // son las facturas del principal y las sucursales
                var sql = @"
                select 
                 top 20
                 ID ""Id"",
                 FECHA_DOCUMENTO ""fechaFactura"",
                 NRO_DOCUMENTO ""NumFolio"",
                 TIPO_DOCUMENTO,
                 RUC ""RucPrincipal"",
                 MONTO ""montoFactura"",
                 PUNTOS ""Puntos"",
                 0 ""NumIntentos"",
                 '' ""RespWS""
                from
                 JBP_DOCUMENTOS_AJUSTE_PTK
                where 
                 ENVIADO=false
                ";
                var bc = new BaseCore();
                var dt = bc.GetDataTableByQuery(sql);
                return new DocumentosPtkBusiness().GetListDocumentosPtkFromDt(dt);
            }
            catch (Exception e)
            {
                e = ExceptionManager.GetDeepErrorMessage(e, ExceptionManager.eCapa.Business);
                throw e;
            }
        }

        private void EnviarFacturas()
        {
            var facturas = new FacturaBusiness().GetFacturasParticipantesToSendPromotick();
            new SendDocWsPtk().SendDocumentosToPromotickWS(facturas);
        }
        private void EnviarNotasCredito()
        {
            var notasCredito = new NotaCreditoBusiness().GetNotasCreditoParticipantesToSendPromotick();
            notasCredito = notasCredito.Take(2).ToList();//solo para pruebas
            new SendDocWsPtk().SendDocumentosToPromotickWS(notasCredito);
        }
        /// <summary>
        /// Cuando se hace alguna modificación de un participante en sap
        /// se dispara un trigger que levanta la bandera de sincronización
        /// para registrar la actualización en la bdd de ptk
        /// </summary>
        
        public List<DocumentoPromotickMsg> GetListDocumentosPtkFromDt(DataTable dt, eTipoDocumentoPtk tipoDocumentoPtk=eTipoDocumentoPtk.NoDefinido)
        {
            var bc = new BaseCore();
            var ms = new List<DocumentoPromotickMsg>();
            foreach (DataRow dr in dt.Rows)
            {
                var factura = new DocumentoPromotickMsg
                {
                    id = bc.GetInt(dr["Id"]),
                    fechaFactura = dr["fechaFactura"].ToString(),
                    numFactura = dr["NumFolio"].ToString(),
                    numDocumento = dr["RucPrincipal"].ToString(),
                    montoFactura = bc.GetInt(dr["montoFactura"]),
                    puntos = bc.GetInt(dr["Puntos"]),
                    numIntentosTx = bc.GetInt(dr["NumIntentos"]),
                    RespuestaWS = dr["RespWS"].ToString()
                };

                if (tipoDocumentoPtk != eTipoDocumentoPtk.NoDefinido)
                    factura.EnumTipoDocumento = tipoDocumentoPtk;
                else {
                    switch (dr["TIPO_DOCUMENTO"].ToString()) {
                        case "ajusteFacturaVentas":
                            factura.EnumTipoDocumento = eTipoDocumentoPtk.AjusteFacturaVentas;
                            break;
                        case "ajusteNotaCredito":
                            factura.EnumTipoDocumento = eTipoDocumentoPtk.AjusteNotaCredito;
                            break;
                    }
                }
                ms.Add(factura); ;
            }
            return ms;
        }
    }
}
