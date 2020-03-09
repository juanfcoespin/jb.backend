using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbp.core
{
    public class RetencionesCore: BaseCore
    {
        /// <summary>
        /// trae las retenciones por autorizar con los meses separados por comas
        /// </summary>
        /// <param name="mesesRetencion">Ej: 10,11</param>
        /// <returns></returns>
        public static string SqlGetRetencionesPorAutorizar(string mesesRetencion) {
            //consulta las retenciones del mes actual y el anterior
            var ms= string.Format(@"
            select distinct * from 
            (
            SELECT 
              R.DRETAUT,
	            R.DRETCPRV RUC,
	            T.DESCRIPTION AS PROVEEDOR,
	            to_char(R.DRETFRET,'dd/mm/yyyy') FECHARET,
	            TO_CHAR( R.DRETFRET, 'MM' ) || '/' || TO_CHAR( R.DRETFRET, 'YYYY' ) AS PERFISCAL,
	            R.DRETNSR NUMRET1,
	            R.DRETNUM NUMRET2,
	            DET.PRIMADDRLINE1 || ' - ' || DET.PRIMADDRCITY AS DIRECCION,
	            DET.PRIMADDRPHONE AS TELEFONO,
	            R.DRETDESC AS RAZON,
	            DRETNSRF NUMFACT1,
	            DRETFPRTN NUMFACT2,
                DRETNSRF || '-' || DRETFPRTN FACTURA,
	            DET.PRIMADDREMAIL AS MAIL 
            FROM
	            GMS.RFDRET R left outer join
				--facturas no anuladas
				JBPVW_DOCUMENTOFUENTE t1 on t1.NUMERODOCUMENTO= R.DRETNSRF||'-'||R.DRETFPRTN and t1.IDESTADO<>3 left outer join
	            FDTRADINGPARTNE T on R.DRETCPRV = T.TP left outer join
	            fdORDERfromroLE AFACT on T.OBJECTID = AFACT.PARENTOBJECTID left outer join
	            FDCONTACT DET on AFACT.CONTACTOBJECTID = DET.OBJECTID
            WHERE
                R.DRETSTAT='I'
	            and R.DRETAUT is null
                and DRETSER in (121423)
            ORDER BY
	            R.DRETFRET,
	            T.DESCRIPTION,
	            R.DRETNSR,
	            R.DRETNUM
            )", mesesRetencion);
            return ms;
        }

        public static string SqlProductosByRucAndNumFactura(string ruc, string prefijoFactura, string secuencialFactura)
        {
      
            var ms= string.Format(@"
                SELECT
                    distinct
                 A.IMPTIP AS TIPIMP,
	             B.IMPCOD AS CODIMP,
	             A.IMP1COD CODIMPR,
                 DRETCIMP DESCIMP,
	             DRETBIMP BASEIMP,
                 DRETVRET PORCIMP,
	             DRETFPRTN NUMFACT2,
                 DRETNSRF NUMFACT1,
	             --DRETTAXID, 
	             substr(DRETNSRF, 1, 3) || substr(DRETNSRF, 5, 3) || DRETFPRTN AS NUMFACTURA,
                 --B.IMPPOR AS PORCENTAJE
                 round((DRETVRET/DRETBIMP)*100,0) PORCENTAJE
                FROM
                 GMS.RFDRET R,
                 CORD.SRIIMP1 A,
                 CORD.SRIIMP B
                WHERE
                 R.DRETSTAT<>'N' --que la linea de la retencion no esté anulada
                 and R.DRETCPRV = '{0}'
                 AND R.DRETNSRF = '{1}'
                 AND R.DRETFPRTN = '{2}'
                 AND TRIM(A.IMP1COD ) = TRIM(R.DRETCIMP)
                 AND A.IMPTIP = B.IMPTIP
                 AND A.IMPCOD = B.IMPCOD
                ", ruc,prefijoFactura, secuencialFactura);
            return ms;
        }

        public static string SqlRetencionesPorAsignarNumero(int diffDaysFactRet)
        {
            var ms= string.Format(@"
                  select 
	                 distinct
	                 t0.DRETAUT,
	                 t0.DRETSTAT,
	                 to_char(t0.DRETFRET,'dd/mm/yyyy') fechaRetencion,
	                 to_char(t1.FECHAEMISION,'dd/mm/yyyy') fechaFacturaCompra,
	                 t0.DRETFRET-t1.FECHAEMISION dias,
	                 t1.estado,
	                 t0.DRETCPRV ruc,
	                 t0.DRETNSRF prefijoFactura,
	                 t0.DRETFPRTN secuenciaFactura,
	                 t0.DRETNSR prefijoRetencion,
	                 t0.DRETNUM numRetencion
	                from GMS.RFDRET t0 left outer join
	                 JBPVW_DOCUMENTOFUENTE t1 on 
	                  t1.NUMERODOCUMENTO like t0.DRETNSRF||'-'||t0.DRETFPRTN||'%' --cuando anulan la factura de compra agregan cualquier caracter en el protean
		                and t1.IDESTADO<>3 --anulado
	                where
	                 t0.DRETSER in (114516,114517)
                     /*t0.dretstat='R'
	                 and t1.tipoDocumento='Factura a Pagar'
	                 and to_char(t0.DRETFRET, 'yyyy') = to_char(sysdate, 'yyyy')
	                 and to_char(t0.DRETFRET, 'mm') in (to_char(sysdate, 'mm')-2, to_char(sysdate, 'mm')-1,to_char(sysdate, 'mm')) 
	                 and t0.DRETAUT is null
	                 and t0.DRETNUM=0
	                 and t0.DRETFRET-t1.FECHAEMISION<={0}*/", diffDaysFactRet);
            return ms;
        }
    }
}
