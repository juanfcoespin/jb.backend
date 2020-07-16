using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using jbp.msg;
using TechTools.Exceptions;
using TechTools.Core.Hana;
using System.Data;


namespace jbp.business.hana
{
    public class FacturaBusiness
    {
        /// <summary>
        /// Trae las facturas que están pendientes por enviar a promotick
        /// por el servicio web expuesto por ellos desde el 01 de marzo del 2020
        /// (de esta fecha hacia atrás ya se envió la información por otros medios)
        /// 
        /// Se envian paquetes de 20 facturas en cada iteración (para evitar posibles problemas en el WS)
        /// </summary>
        /// <returns></returns>
        public List<DocumentoPromotickMsg> GetFacturasParticipantesToSendPromotick()
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
                 ""JbpVw_FacturasToSendPtk""
                --where ""Id""=12741
                ";
                var bc = new BaseCore();
                var dt = bc.GetDataTableByQuery(sql);
                return new DocumentosPtkBusiness().GetListDocumentosPtkFromDt(dt, eTipoDocumentoPtk.FacturaDeVenta);
            }
            catch (Exception e)
            {
                e = ExceptionManager.GetDeepErrorMessage(e, ExceptionManager.eCapa.Business);
                throw e;
            }
        }
    }
}
