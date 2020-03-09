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
            
            return string.Format(@"
            	select
	                p.codRecurso CODIGO,
	                p.descripcion as Producto,
	                ord.codOrden ORDEN,
	                p.unidad UM,
	                p.ID OBJRSC,		
	                df.precioUnitario PRECIOUNIT,
	                df.ordenlinea ORDLIN,
	                sum(df.cantidad) CANTIDAD,
	                sum(df.DESCUENTO) DSCTO,
	                sum(df.montoBruto) - sum(df.DESCUENTO) NETOSINIMP,
	                sum(df.impuestos) IMPUESTO
                from
                 JBPVW_FACTURARESUMEN fr inner join
                 jbpvw_ordenfactura ord on ord.idFactura=fr.id inner join
                 JBPVW_detalleFactura df on df.idOrden=ord.id inner join
                 JBPVW_PRODUCTO p on df.idProducto=p.id inner join
                 JBPVW_SOCIONEGOCIO sn on fr.RUCSOCIONEGOCIO=sn.ruc
                where
	                fr.id={0}
                group by
	                p.codRecurso,
	                p.descripcion,
	                ord.codOrden,
	                p.unidad,
	                p.ID,
	                df.precioUnitario,
	                df.ordenlinea
	        ", idFactura);        
        }
    }
}
