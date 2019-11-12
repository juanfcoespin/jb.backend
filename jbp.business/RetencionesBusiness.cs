using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using jbp.core;

namespace jbp.business
{
    public class RetencionesBusiness:BaseBusiness
    {
        public static DataTable GetRetencionesToUpdateNum() {
            var ms = new DataTable();
            var sql = @"
            select 
             distinct
             t0.DRETFRET fechaRetencion,
             t0.DRETNSRF||'-'|| t0.DRETFPRTN CodFactura,
             t0.DRETNSR prefijoRetencion,
             t0.DRETNUM numRetencion,
             t0.NUM_RET_MODIFICADO
            from GMS.RFDRET t0 
            where 
             to_char(t0.DRETFRET, 'yyyy-mm' ) in ('2019-09','2019-10') 
             and t0.DRETNSR='001-050'
             and t0.DRETNUM<100 --26844
             and t0.DRETAUT is null
            order by 
             t0.DRETNUM, t0.DRETNSRF||'-'|| t0.DRETFPRTN
            ";
            ms = new BaseCore().GetDataTableByQuery(sql); 
            return ms;
        }
        public void UpdateNumRetencionSet(int nuevoNumeroRetencion) {
            var dt = GetRetencionesToUpdateNum();
            if (dt != null && dt.Rows.Count > 0) {
                foreach (DataRow dr in dt.Rows) {
                    var codFactura = dr["CODFACTURA"].ToString();
                    var prefijoRet = dr["prefijoRetencion"].ToString();
                    var numRet = dr["numRetencion"].ToString();
                    var sql = string.Format(@"
                        update GMS.RFDRET
                        set NUM_RET_MODIFICADO={0}
                        where
                         DRETNSRF||'-'|| DRETFPRTN ='{1}'
                         and DRETNSR='{2}'
                         and DRETNUM={3}
                    ", nuevoNumeroRetencion, codFactura,prefijoRet,numRet);
                    new BaseCore().Execute(sql);
                    NotifyMsg(string.Format(
                        "Asignado en número de retención {0} a la factura {1}",
                        nuevoNumeroRetencion,codFactura));
                    nuevoNumeroRetencion++;
                }
            }
        }
        public DataTable GetRetencionesPorAutorizar(int diffDaysFactRet) {
            AsignarNumSecuencialEnRetencionesNuevas(diffDaysFactRet);
            return new BaseCore().GetDataTableByQuery(RetencionesCore.SqlGetRetencionesPorAutorizar());
        }

        private void AsignarNumSecuencialEnRetencionesNuevas(int diffDaysFactRet)
        {
            var dtRetPorAsignarNumero = new BaseCore().GetDataTableByQuery(
                RetencionesCore.SqlRetencionesPorAsignarNumero(diffDaysFactRet)    
            );
            if (dtRetPorAsignarNumero != null && dtRetPorAsignarNumero.Rows.Count > 0) {
                foreach (DataRow dr in dtRetPorAsignarNumero.Rows) {
                    int numRetencion = GetSiguienteNumRetencion();
                    var fechaRet = dr["fechaRetencion"].ToString();
                    var ruc= dr["ruc"].ToString();
                    var prefijoFactura = dr["prefijoFactura"].ToString();
                    var secuenciaFactura = dr["secuenciaFactura"].ToString();
                    if (ActualizarNumRetencion(numRetencion, fechaRet, ruc, prefijoFactura, secuenciaFactura)) {
                        RegistrarUltimoNumRetencion(numRetencion);
                    }
                }
                
            }
        }

        private static void RegistrarUltimoNumRetencion(int numRetencion)
        {
            var sql = string.Format("UPDATE GMS.TBL_NUM_RET_SRI SET SECRET = {0}", numRetencion);
            new BaseCore().Execute(sql);
        }

        private static int GetSiguienteNumRetencion()
        {
            var sql = "select secret FROM GMS.TBL_NUM_RET_SRI";
            return new BaseCore().GetIntScalarByQuery(sql)+1;
        }

        public static object GetProductosByRucAndNumFactura(string ruc, string prefijoFactura, string secuencialFactura)
        {
            return new BaseCore().GetDataTableByQuery(
                RetencionesCore.SqlProductosByRucAndNumFactura(ruc, prefijoFactura, secuencialFactura)
            );
        }

        private static bool ActualizarNumRetencion(int numRetencion, string fechaRetencion, string ruc, string prefijoFactura, string secuencialFactura)
        {
            try
            {
                var sql = string.Format(@"
                    UPDATE GMS.RFDRET R 
                     SET R.DRETNSR = '001-050',
                     R.DRETNUM = {0},
                     R.DRETSTAT = 'I'
                    WHERE 
                     R.DRETFRET = TO_DATE('{1}', 'DD/MM/YYYY')
                     AND R.DRETSTAT = 'R'
                     AND R.DRETCPRV = '{2}'
                     AND R.DRETNSRF  = '{3}'
                     AND R.DRETFPRTN = '{4}'
                ", numRetencion, fechaRetencion, ruc, prefijoFactura, secuencialFactura);
                new BaseCore().Execute(sql);
                
                return true;
            }
            catch
            {
                return false;
            }
            
        }
        public static void RegistrarAutorizacionSRI(string numAutorizacion, string idDocumento)
        {
            var sql = string.Format(@"
                UPDATE GMS.RFDRET SET DRETAUT = '{0}',DRETSTAT = 'I' WHERE DRETNUM = {1}",
                numAutorizacion, idDocumento);
            new BaseCore().Execute(sql);
        }
    }
}
