using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using jbp.msg;
using TechTools.DelegatesAndEnums;

namespace jbp.core
{
    public class FacturaCore:BaseCore
    {
        public static int LastIdFactura = 0;
        public static string SqlGetCodFacturaById(string idFactura)
        {
            return string.Format(@"
                select CODFACTURA from jbpvw_FacturaResumen
                where id={0}
            ", idFactura);
        }
        public static string SqlDocsFinancierosByType(eTipoDocFuente tipoDocFuente)
        {
            var ms = @"
             SELECT
                distinct
                S.ObjectID IDFACT,
                O.ObjectID IDORDEN,
                S.ORDERUK ORDEN,
                S.InvcUKInvc FACTURA,
                S.INVCSITE SITIO,
                S.InvcDate FechaFact,
                C.Description RazonSocialCli,
                S.InvcToTP RUC, 
                S.NetTaxBAmt TotalSinImp,
                S.DiscBaAmt Descuento,
                S.TaxBaseAmt Impuestos,
                S.NetTtlBAmt ImporteTotal,
                'DOLAR' MONEDA,
                S.PAYTERMS TERMPAGO,
                C.KNOWNASNAME CONOCIDOCOMO,
                S.SELLLOB CIUFACT, 
                S.INVCTOADDRLINE1 DIRECCION,
                S.INVCTOADDRCITY CIUDAD,
                S.INVCTOADDRPHONE TELEFONO,
                S.INVCTOCTCT CONTACTO,
                S.INVCDUEDATE FECHVENC,
                AO.AOVENDEDO AS VENDEDOR,
                PRIMADDREMAIL AS MAIL";
            if (tipoDocFuente == eTipoDocFuente.factura)
                ms += ",G.DOCGUIA GUIA ";
            if ((tipoDocFuente == eTipoDocFuente.NotasCredito))
            {
                ms += @", l.originvcinvc AS FACTAPP
                        , T.aocomentar AS COMENTARIO ";
            }
                ms +=@" 
                From
                    COINVCSUMRY S,
                    FDTRADINGPARTNE C,
                    COINVCORDER O,
                    FDADDON AD,
                    AOADICIONALES AO,
                    FDINVOICETOROLE AFACT,
                    FDCONTACT DET";
            if ((tipoDocFuente == eTipoDocFuente.NotasCredito)) {
                ms += @", coinvcline l
                        , AOTRANSPORTE T";
            }
            if (tipoDocFuente == eTipoDocFuente.factura)
                ms += ", GMS.TBL_GUIAS_REMISION G ";
            ms += @" 
                Where 
                -- 0 FACTURA, 1 N/C, 2 N/D facturaServicios, 3 devolucion 
                ";
            if (tipoDocFuente == eTipoDocFuente.NotasCredito)
            {
                ms += @"
                    S.INVCTYPE IN ( 1, 3 ) 
                    AND SUBSTR( S.InvcUKInvc, 1, 7 ) = 'C01-010'
                ";
            }
            else
                ms += "S.INVCTYPE = " + ((int)tipoDocFuente).ToString();

            if (tipoDocFuente == eTipoDocFuente.factura || tipoDocFuente == eTipoDocFuente.facturaServicios)
                ms += " And SUBSTR(S.InvcUKInvc,1,7) In ('001-010','001-020','002-010')";
            ms +=@"  
                    AND to_char(S.InvcDate,'yyyy-mm') = to_char(sysdate,'yyyy-mm')
                    AND S.InvcToTPObjectID = C.OBJECTID
                    AND S.ObjectID = O.PARENTOBJECTID
                    AND C.OBJECTID = AD.PARENTOBJECTID
                    AND AD.OBJECTID = AO.FDADDONID
                    AND C.OBJECTID = AFACT.PARENTOBJECTID
                    AND AFACT.CONTACTOBJECTID = DET.OBJECTID";
            if (tipoDocFuente == eTipoDocFuente.factura)
                ms += " AND S.ObjectID = G.FOBJECID";
            if (tipoDocFuente == eTipoDocFuente.factura || tipoDocFuente == eTipoDocFuente.facturaServicios)
                ms += " AND AO.AOVENDEDO NOT IN ('CISNEROS TORRES LOLA') -- exportaciones";
            if (tipoDocFuente == eTipoDocFuente.NotasCredito)
            {
                ms += @" AND o.objectid = l.parentobjectid
                         AND O.ORDERUKOBJECTID = T.PARENTOBJECTID";
            }
                
            ms += @"
                AND S.CMNT IS NULL --  SOLO FACTURAS QUE NO TIENEN NUM DE AUTORIZACION del SRI
                AND S.InvcUKInvc IN ('001-010-0052970') --Buscar Factura Expecifica
            ";
            return ms;
        }
        public static string SqlObservacionFacturaById(string idFactura) {
            var ms = string.Format(@"
                SELECT
                    AOCOMENTAR AS OBS 
                FROM
                    COINVCSUMRY S,
                    coOrderSumry o,
                    FDADDON AD,
                    AOTRANSPORTE AO 
                WHERE 
                    S.objectid = {0}
                    AND S.ORDERUKOBJECTID = O.OBJECTID
                    AND o.objectid = AD.PARENTOBJECTID
                    AND AD.OBJECTID = AO.FDADDONID
            ", idFactura);
            return ms;
        }
        public static string SqlBonificacionByIdFacturaCodRecurso(string idFactura, string codRecurso) {
            return string.Format(@"
                     SELECT
                            L.IVDUSQTY AS SQ_CANTIDAD 
                        From coInvcOrder O, COINVCLINE L, FDBASRESC R
                        Where
                            O.PARENTOBJECTID = '{0}'
                            And L.RESC = '{1}'
                            And L.PARENTOBJECTID = O.OBJECTID
                            AND L.RESCOBJECTID = R.OBJECTID
                            And L.RESCSITE = R.RESOURCEUKSITE
                            AND L.NETTAXBAMT = '0'
                ", idFactura, codRecurso);
            
        }
        public static string SqlFacturasPorAprobar(eTipoDocFuente tipoDocumento) {
            var sql = @"
                select
                 fr.id idFact,
                 o.id idOrden,
                 o.codOrden,
                 fr.codFactura factura,
                 fr.SITIO,
                 fr.FECHAFACTURA fechaFact,
                 sn.nombre razonSocialCli,
                 fr.RUCSOCIONEGOCIO ruc,
                 fr.MONTOBRUTO,
                 fr.DESCUENTO,
                 fr.IMPUESTO impuestos,
                 fr.MONTO importeTotal,
                 'DOLAR' MONEDA,
                 fr.TERMINOPAGO termpago,
                 sn.conocidoComo,
                 fr.ciudadFactura ciuFact,
                 fr.DIRECCIONFACTURA direccion,
                 fr.ciudad,
                 fr.telefono,
                 fr.CLIENTE contacto,
                 fr.fechaVencimiento fechaVenc,
                 v.vendedor,
                 c.email mail";
                if (tipoDocumento == eTipoDocFuente.factura)// solo aqui se extraen las guias
                    sql += ", g.DOCGUIA numGuia";
                sql +=@"
                from	             JBPVW_FACTURARESUMEN fr left join
	             jbpvw_ordenFactura o on o.idFactura=fr.id left join
	             JBPVW_SOCIONEGOCIO sn on fr.RUCSOCIONEGOCIO=sn.ruc left join
	             jbpvw_vendedor v on v.idSocioNegocio=sn.id left join
	             jbpvw_facturarA fa on fa.idSocioNegocio=sn.id left join
	             JBPVW_CONTACTO c on fa.idContacto=c.id";
                if (tipoDocumento == eTipoDocFuente.factura)// solo aqui se extraen las guias
                    sql += " left join GMS.TBL_GUIAS_REMISION g on g.FOBJECID=fr.id ";
                sql += @"
                where 
                 
                 fr.idTipo={0}
	             and v.vendedor not in ('CISNEROS TORRES LOLA') -- no se toman en cuenta exportaciones
	             and SUBSTR(fr.CODFACTURA,1,7) In ('001-010','001-020','002-010') --solo facturas
                 --and to_char(fr.FECHAFACTURA,'yyyy-mm')=to_char(SYSDATE,'yyyy-mm')
	             and fr.autorizacionSri is null
                 and fr.CODFACTURA in (
                    '001-010-0061832',
                    '001-010-0061833',
                    '001-010-0061834'
) 
            ";
            sql=string.Format(sql, (int)tipoDocumento);
            return sql;
        }

        public static string SqlGetNumGuiaByIdFactura(string idFactura) {
            return string.Format(
                "select DOCGUIA from GMS.TBL_GUIAS_REMISION where FOBJECID={0}", 
                idFactura);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="date">En formato yyyy-mm-dd</param>
        /// <returns></returns>
        public static FacturaPromotickMsg GetFacturaByDate(string date) {
            try
            {
                var ms = new FacturaPromotickMsg();
                var sql = string.Format("");
                return ms;
            }
            catch
            {
                throw;
            }
        }
        public int GetMaxIdFactura() {
            var sql = "select max(id) from JBPVW_FACTURARESUMEN";
            return this.GetIntScalarByQuery(sql);
        }
    }
}
