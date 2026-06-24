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
using System.Xml;

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
        public List<DocumentoPromotickMsg> GetFacturasYNcParticipantesToSendPromotick()
        {
            try
            {
                // son las facturas del principal y las sucursales
                var sql = @"
                select 
                top 500
                 ""Id"",
                ""TipoDocumento"",
                 ""fechaFactura"", --dd/mm/yyyy
                 ""NumFolio"",
                 ""RucPrincipal"",
                 ""montoFactura"",
                 ""puntos"",
                 ""NumIntentos"",
                 ""RespWS""
                from
                 ""JbpVw_FacturasYNCToSendPtk""
                ";
                var bc = new BaseCore();
                var dt = bc.GetDataTableByQuery(sql,null);
                return new DocumentosPtkBusiness().GetListDocumentosPtkFromDt(dt);
            }
            catch (Exception e)
            {
                e = ExceptionManager.GetDeepErrorMessage(e, ExceptionManager.eCapa.Business);
                throw e;
            }
        }

        internal static DatosRelacionadosFacturaPagoMsg GetDatosFactura(int idFactura)
        {
            var bc = new BaseCore();
            var sql=string.Format(@"
                 select
                  ""DocEntry"" ""Id"",
                  ""U_NUM_AUTOR"" ""NumAutorizacion"",
                  ""U_SER_EST"" ""PtoEstablecimiento"",
                  ""U_SER_PE"" ""PtoEmision"",
                  ""DocDate"" ""Fecha""
                 from
                 ""OINV"" 
                 where
                  ""DocEntry""=?

            ");
            var dt = bc.GetDataTableByQuery(sql, new Dictionary<string, object> {
                {"@0", idFactura }
            });
            if (dt != null && dt.Rows.Count > 0)
            {
                var dr = dt.Rows[0];
                return new DatosRelacionadosFacturaPagoMsg
                {
                    Id = bc.GetInt(dr["Id"]),
                    NumAutorizacion = dr["NumAutorizacion"].ToString(),
                    PtoEstablecimiento = dr["PtoEstablecimiento"].ToString(),
                    PtoEmision = dr["PtoEmision"].ToString(),
                    Fecha = dr["Fecha"].ToString(),
                };
            }
            else
                return null;
            
            
        }
        /*
         internal static int GetDocEntryFromFolioNum(int folioNum)
        {
            var bc = new BaseCore();
            var sql=string.Format(@"select ""DocEntry"" from OINV where ""FolioNum""='{0}'",folioNum);
            var ms= bc.GetIntScalarByQuery(sql);
            return ms;
        }
         */

        public static string updateFolioNumFactExportacion(FactExportacionMe me)
        {
            try
            {
                if (!esFacturaExportacion(me))
                    return "Solo se puede asignar el numero de folio a facturas de exportación!!";
                registrarLogActualizacionFolioNum(me);
                var sql = string.Format(@"
                    update OINV
                    set ""FolioNum""=?, ""FolioPref"" = 'FV'
                    where ""DocNum"" = ?;
                ");
                new BaseCore().Execute(sql,new Dictionary<string, object> {
                    {"@0", me.FolioNum },
                    {"@1",me.DocNum }
                });
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
                where ""DocNum""=?
            ");
            var bc = new BaseCore();
            var docNumAnterior = bc.GetScalarByQuery(sql, new Dictionary<string, object> { { "@0", me.DocNum } });
            sql = string.Format(@"
                insert into JBP_LOG_FACTURAS_EXPORTACION(
                    ACTUALIZADOR, DOC_NUM_SAP, NUM_FACTURA_ANTERIOR,
                    NUM_FACTURA_ACTUAL, FECHA_ACTUALIZACION
                )VALUES(
                    ?, ?, ?,
                    ?, current_timestamp
                )
            ");
            new BaseCore().Execute(sql, new Dictionary<string, object> {
                {"@0", me.Actualizador },
                {"@1", me.DocNum },
                {"@2", docNumAnterior },
                {"@3",me.FolioNum }
            });
        }

        internal static List<ValorPagadoMsg> GetPagosByIdFactura(int idFactura)
        {
            var ms = new List<ValorPagadoMsg>();
            var sql = string.Format(@"
                select 
                 t1.""DocNum"",
                 t0.""Pago"",
                 to_char(t1.""Fecha"", 'yyyy-mm-dd') ""Fecha""
                from
                 ""JbpVw_PagosRecibidosLinea"" t0 inner join
                 ""JbpVw_PagosRecibidos"" t1 on t1.""Id"" = t0.""IdPagoRecibido"" inner join
                 ""JbpVw_Factura"" t2 on t2.""Id"" = t0.""IdFactura""
                where
                 upper(t1.""Comentario"") not like '%RET%'--que no sea una retencion
                 and t2.""Id"" = ?
            ");
            var bc = new BaseCore();
            var dt = bc.GetDataTableByQuery(sql, new Dictionary<string, object> {
                {"@0", idFactura }
            });
            foreach (DataRow dr in dt.Rows) {
                ms.Add(new ValorPagadoMsg {
                    DocNum = dr["DocNum"].ToString(),
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
                 and t2.""DocNum"" = ?
            ");
            var bc = new BaseCore();
            var dt = bc.GetDataTableByQuery(sql, new Dictionary<string, object> {
                {"@0", docNumFactura }
            });
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
                from  ""JbpVw_Factura"" where ""DocNum"" = ?;
                ");
            var lugar=new BaseCore().GetScalarByQuery(sql, 
                new Dictionary<string, object> { { "@0", me.DocNum } }
            );
            return lugar == "FV_EXPP";
        }

        internal static int GetIdChequeProtestadoByDocNum(string numDoc)
        {
            var sql = string.Format(@"select ""DocEntry"" from OVPM where ""DocNum""=?", numDoc);
            return new BaseCore().GetIntScalarByQuery(sql, new Dictionary<string, object> { { "@0", numDoc } });
        }

        internal static List<ValorPagadoMsg> GetPagosBorradorByIdFactura(int idFactura)
        {
            var ms = new List<ValorPagadoMsg>();
            var sql = string.Format(@"
                select 
                 ""DocNum"",
                 to_char(""Fecha"",'yyyy-mm-dd') ""Fecha"",
                 ""Total""
                from
                 ""JbpVw_PagosBorrador"" t0 inner join
                 ""JbpVw_PagosBorradorLinea"" t1 on t1.""IdPago""=t0.""Id""
                where 
                 t1.""IdFactura"" = ?
            ");
            var bc = new BaseCore();
            var dt = bc.GetDataTableByQuery(sql,new Dictionary<string, object> { { "@0", idFactura } });
            foreach (DataRow dr in dt.Rows)
            {
                ms.Add(new ValorPagadoMsg
                {
                    DocNum = dr["DocNum"].ToString(),
                    Valor = bc.GetDecimal(dr["Total"]),
                    Fecha = dr["Fecha"].ToString()
                });
            }
            return ms;
        }
    }
}
