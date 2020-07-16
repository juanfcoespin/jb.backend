using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using jbp.msg;
using TechTools.Core.Hana;
using System.Data;
using TechTools.Exceptions;

namespace jbp.business.hana
{
    public class NotaCreditoBusiness
    {
        internal List<DocumentoPromotickMsg> GetNotasCreditoParticipantesToSendPromotick()
        {
            try
            {
                // son las facturas del principal y las sucursales
                var sql = @"
                select 
                 --top 25
                 ""Id"",
                 ""fechaFactura"",
                 ""NumFolio"",
                 ""RucPrincipal"",
                 ""montoFactura"",
                 ""Puntos"",
                 ""NumIntentos"",
                 ""RespWS""
                from
                 ""JbpVw_NcToSendPtk""
                ";
                var bc = new BaseCore();
                var dt = bc.GetDataTableByQuery(sql);
                return new DocumentosPtkBusiness().GetListDocumentosPtkFromDt(dt, eTipoDocumentoPtk.NotaDeCredito);
            }
            catch (Exception e)
            {
                e = ExceptionManager.GetDeepErrorMessage(e, ExceptionManager.eCapa.Business);
                throw e;
            }
        }

        
    }
}
