using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using jbp.msg;
using TechTools.Exceptions;
using TechTools.Core.Hana;
using System.Data;
using jbp.msg.sap;

namespace jbp.business.hana
{
    public class FacturaBusiness
    {
        /// <summary>
        /// Trae las facturas que están pendientes por enviar a promotick
        /// por el servicio web expuesto por ellos desde el 01 de marzo del 2020
        /// (de esta fecha hacia atrás ya se envió la información por otros medios)
        /// 
        /// Se envian paquetes de 50 facturas en cada iteración (para evitar posibles problemas en el WS)
        /// </summary>
        /// <returns></returns>
        public List<DocumentoPromotickMsg> GetFacturasParticipantesToSendPromotick()
        {
            try
            {
                // son las facturas del principal y las sucursales
                var sql = @"
                select 
                top 150
                 ""Id"",
                ""TipoDocumento"",
                 ""fechaFactura"", --dd/mm/yyyy
                 ""NumFolio"",
                 ""RucPrincipal"",
                 ""montoFactura"",
                 ""Puntos"",
                 ""NumIntentos"",
                 ""RespWS""
                from
                 ""JbpVw_FacturasToSendPtk""
                --where
                 --""NumFolio""='001-010-000103191'
                 --""RucPrincipal""='0101766582001'   
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

        internal static int GetDocEntryFromFolioNum(int folioNum)
        {
            var bc = new BaseCore();
            var sql=string.Format(@"select ""DocEntry"" from OINV where ""FolioNum""='{0}'",folioNum);
            var ms= bc.GetIntScalarByQuery(sql);
            return ms;
        }

        public static string updateFolioNumFactExportacion(FactExportacionMe me)
        {
            try
            {
                if (!esFacturaExportacion(me))
                    return "Solo se puede asignar el numero de folio a facturas de exportación!!";
                registrarLogActualizacionFolioNum(me);
                var sql = string.Format(@"
                    update OINV
                    set ""FolioNum""='{0}', ""FolioPref"" = 'FV'
                    where ""DocNum"" = {1};
                ", me.FolioNum, me.DocNum);
                new BaseCore().Execute(sql.ToString());
                return "ok";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        private static void registrarLogActualizacionFolioNum(FactExportacionMe me)
        {
            var sql = string.Format(@"
                SELECT ""FolioNum"" from OINV
                where ""DocNum""={0}
            ", me.DocNum);
            var bc = new BaseCore();
            var docNumAnterior = bc.GetScalarByQuery(sql);
            sql = string.Format(@"
                insert into JBP_LOG_FACTURAS_EXPORTACION(
                    ACTUALIZADOR, DOC_NUM_SAP, NUM_FACTURA_ANTERIOR,
                    NUM_FACTURA_ACTUAL, FECHA_ACTUALIZACION
                )VALUES(
                    '{0}', {1}, {2},
                    {3}, current_timestamp
                )
            ", me.Actualizador, me.DocNum, docNumAnterior,
            me.FolioNum);
            new BaseCore().Execute(sql);
        }

        internal static List<ValorPagadoMsg> GetPagosByDocNum(int docNum)
        {
            var ms = new List<ValorPagadoMsg>();
            var sql = string.Format(@"
                select 
                 t0.""Pago"",
                 to_char(t1.""Fecha"", 'yyyy-mm-dd') ""Fecha""
                from
                 ""JbpVw_PagosRecibidosLinea"" t0 inner join
                 ""JbpVw_PagosRecibidos"" t1 on t1.""Id"" = t0.""IdPagoRecibido"" inner join
                 ""JbpVw_Factura"" t2 on t2.""Id"" = t0.""IdFactura""
                where
                 upper(t1.""Comentario"") not like '%RET%'--que no sea una retencion
                 and t2.""DocNum"" = {0}
            ", docNum);
            var bc = new BaseCore();
            var dt = bc.GetDataTableByQuery(sql);
            foreach (DataRow dr in dt.Rows) {
                ms.Add(new ValorPagadoMsg {
                    Valor = bc.GetDecimal(dr["Pago"]),
                    Fecha = dr["Fecha"].ToString()
                });
            }
            return ms;
        }

        internal static List<RetencionMsg> GetRetencionesByDocNum(int docNumFactura)
        {
            var ms = new List<RetencionMsg>();
            var sql = string.Format(@"
                select 
                 t0.""Pago"" ""ValorRetencion"",
                 round((t0.""Pago"" / t2.""Total""), 4) * 100 ""%Ret""
                from
                 ""JbpVw_PagosRecibidosLinea"" t0 inner join
                 ""JbpVw_PagosRecibidos"" t1 on t1.""Id"" = t0.""IdPagoRecibido"" inner join
                 ""JbpVw_Factura"" t2 on t2.""Id"" = t0.""IdFactura""
                where
                 upper(t1.""Comentario"") like '%RET%' --que el comentario indique que es una retencion
                 and t2.""DocNum"" = {0}
            ", docNumFactura);
            var bc = new BaseCore();
            var dt = bc.GetDataTableByQuery(sql);
            foreach (DataRow dr in dt.Rows) {
                ms.Add(new RetencionMsg { 
                    Valor = bc.GetDecimal(dr["ValorRetencion"]),
                    PorcentajeRet = bc.GetDecimal(dr["%Ret"])
                });
            }
            return ms;
        }

        private static bool esFacturaExportacion(FactExportacionMe me)
        {
            var sql = string.Format(@"
                select 
                 ""LugarFacturacion""
                from  ""JbpVw_Factura"" where ""DocNum"" = '{0}';
                ",me.DocNum);
            var lugar=new BaseCore().GetScalarByQuery(sql);
            return lugar == "FV_EXPP";
        }
    }
}
