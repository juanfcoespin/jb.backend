﻿using System;
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
    public class DocumentosPtkBusiness:BaseWSPtk { 
        public void EnviarDocumentosAPromotick() {
            try
            {
                var ppb = new ParticipantePtkBusiness();

                if (conf.Default.ptkEnviarFacturasYNc)
                    EnviarFacturasYNC();
            }
            catch (Exception e)
            {
                e = ExceptionManager.GetDeepErrorMessage(e, ExceptionManager.eCapa.Business);
                EnviarPorCorreo("Jbp-Promotick",e.Message);
            }
        }

        public void EnviarAjustes()
        {
            var documentos = new List<DocumentoPromotickMsg>();
            var sql = @"
                select 
                 FECHAFACTURA,
                 NUMFACTURA, 
                 DESCRIPCION,
                 NUMDOCUMENTO,
                 MONTOFACTURA,
                 PUNTOS
                from JBP_TMP_DOCS_PTK
            ";
            var bc = new BaseCore();
            var dt = bc.GetDataTableByQuery(sql);
            foreach (DataRow dr in dt.Rows) {
                var documento = new DocumentoPromotickMsg
                {
                    fechaFactura = dr["FECHAFACTURA"].ToString(),
                    numFactura = dr["NUMFACTURA"].ToString(),
                    numDocumento = dr["NUMDOCUMENTO"].ToString(),
                    montoFactura = bc.GetInt(dr["MONTOFACTURA"]),
                    puntos = bc.GetInt(dr["PUNTOS"]),
                    tipoDocumento = "Ajuste"
                };
                //por defecto en descripcion se inyecta el tipo de documento
                documento._customDescription = true;
                documento._description = dr["DESCRIPCION"].ToString();
                documentos.Add(documento); 
            }
            var obj = new SendDocWsPtk();
            obj.InsertarDocumentosAEnviar(documentos);
            obj.SendDocumentosToWS(documentos, false); // true en el segundo parametro cuando es NC 
        }

        public void EnviarAceleradores(string periodo)
        {
            var aceleradores = new promotickBusiness().GetAceleradoresToSend();
            new SendDocWsPtk().SendAceleradoresToPromotickWS(aceleradores, periodo);
        }
       
        private void EnviarFacturasYNC()
        {
            var facturas = new FacturaBusiness().GetFacturasYNcParticipantesToSendPromotick();
            new SendDocWsPtk().SendDocumentosToPromotickWS(facturas);
        }

        public void EnviarNotasCreditoManuales()
        {
            var notasCredito = new NotaCreditoBusiness().GetNCManualesParticipantesToSendPromotick();
            new SendDocWsPtk().SendDocumentosToPromotickWS(notasCredito);
        }
        /// <summary>
        /// Cuando se hace alguna modificación de un participante en sap
        /// se dispara un trigger que levanta la bandera de sincronización
        /// para registrar la actualización en la bdd de ptk
        /// </summary>

        public List<DocumentoPromotickMsg> GetListDocumentosPtkFromDt(DataTable dt)
        {
            var bc = new BaseCore();
            var ms = new List<DocumentoPromotickMsg>();
            foreach (DataRow dr in dt.Rows)
            {
                var factura = new DocumentoPromotickMsg
                {
                    id = bc.GetInt(dr["Id"]),
                    tipoDocumento= dr["TipoDocumento"].ToString(),

                    fechaFactura = dr["fechaFactura"].ToString(),
                    numFactura = dr["NumFolio"].ToString(),
                    numDocumento = dr["RucPrincipal"].ToString(),
                    montoFactura = bc.GetInt(dr["montoFactura"]),
                    puntos = bc.GetInt(dr["Puntos"]),
                     
                    numIntentosTx = bc.GetInt(dr["NumIntentos"]),
                    RespuestaWS = dr["RespWS"].ToString()
                };
                if(factura.tipoDocumento == "notaCreditoManual") {
                    factura._customDescription = true;
                    factura._description = dr["descripcion"].ToString();
                }
                
                ms.Add(factura);
            }
            return ms;
        }
    }
}
