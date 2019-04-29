using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbp.core
{
    public class DetalleFacturaCore
    {
        public static string SqlDetalleFacturaByIdFactura(string idFactura) {
            //EB_ver2:CAMBIO DE CONSULTA PRA EL DETALLE DE FACTURAS
            //DETALLE DE LOS PRODUCTOS DE LA FACTURA SIN LOTES
            return string.Format(@"
            SELECT
                    SQ_COD AS CODIGO,
                    SQ_DESCRIPT AS PRODUCTO,
                    SUM (SQ_CANTIDAD) AS CANTIDAD,
                    SQ_PRECIOUNIT AS PRECIOUNIT,
                    SUM(SQ_DESCU) AS DSCTO,
                    SUM(SQ_NETOSINIMP) As NETOSINIMP,
                    SQ_ORDLIN As ORDLIN,
                    SQ_OBJRSC As OBJRSC,
                    SQ_ORDEN As ORDEN,
                    SUM (SQ_IMPUESTO) As IMPUESTO,
                    SQ_UM As UM
                FROM
                    (
                        Select
                            L.RESC As SQ_COD,
                            R.DESCRIPTION As SQ_DESCRIPT,
                            L.IVDUSQTY As SQ_CANTIDAD,
                            L.UNITPRICECURAMT As SQ_PRECIOUNIT,
                            L.DISCBAAMT As SQ_DESCU,
                            L.NETTAXBAMT As SQ_NETOSINIMP,
                            L.ORDDEL,
                            L.ORDLIN As SQ_ORDLIN,
                            L.RESCOBJECTID As SQ_OBJRSC,
                            O.ORDERUK As SQ_ORDEN,
                            L.TAXBASEAMT As SQ_IMPUESTO, 
                            L.UNITPRICEUM As SQ_UM
                        FROM
                            coInvcOrder O, COINVCLINE L, FDBASRESC R
                        WHERE
                            O.PARENTOBJECTID = {0}
                            And L.PARENTOBJECTID = O.OBJECTID
                            And L.RESCOBJECTID = R.OBJECTID
                            And L.RESCSITE = R.RESOURCEUKSITE
                    )
                    GROUP BY
                        SQ_COD, SQ_DESCRIPT, SQ_PRECIOUNIT, SQ_ORDLIN, SQ_OBJRSC, SQ_ORDEN, SQ_UM
                    ORDER BY SQ_COD
            ",idFactura);        
        }
    }
}
