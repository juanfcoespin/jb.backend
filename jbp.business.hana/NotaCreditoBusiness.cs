using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using jbp.msg;
using TechTools.Core.Hana;
using System.Data;
using TechTools.Exceptions;
using jbp.msg.sap;
using System.Threading;
using jbp.core.sapDiApi;

namespace jbp.business.hana
{
    public class NotaCreditoBusiness
    {
        internal List<DocumentoPromotickMsg> GetNCManualesParticipantesToSendPromotick()
        {
            try
            {

                /* ojo: para una nueva carga hay que eliminar los datos de la tabla JBP_NC_MANUALES
                 
                previo a cargar las nuevas NC manuales a enviar.
                
                ver query: Catálogo de servicios\Promotick\3. Operacion del Servicio\queries\Documentos\notasCredito\envioNcManuales.sql
                
                son las facturas del principal y las sucursales

                - para saber el factor de conversión (si es 4 o 3 en dependencia si el participante es tipo A o B) se cruza la data con el query:
                  en el queryManager de sap: JbPromotick\JbParticipantesPuntos

                - Verificar log: select * from JBP_LOG_ENVIO_DOCUMENTOS_PTK where  to_char(FECHA_TX, 'yyyy-mm-dd')='2024-06-26'

                */
                var sql = @"
                select 
                 0 ""Id"",
                 'notaCreditoManual' ""TipoDocumento"",
                 FECHA_FACTURA ""fechaFactura"",
                 NUM_FOLIO ""NumFolio"",
                 RUC_PRINCIPAL ""RucPrincipal"",
                 -1*abs(MONTO_FACTURA) ""montoFactura"",
                 -1*abs(PUNTOS) ""Puntos"",
                 DESCRIPCION ""descripcion"",
                 0 ""NumIntentos"",
                 null ""RespWS""
                from
                 JBP_NC_MANUALES
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
     
    }
}
