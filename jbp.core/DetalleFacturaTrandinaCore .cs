using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using jbp.msg;
using TechTools.Exceptions;

namespace jbp.core
{
    public class DetalleFacturaTrandinaCore : BaseCoreEntity<DetalleFacturaTrandinaMsg>
    {
        public DetalleFacturaTrandinaCore() {
            this.TranslateDataToListEvent +=  (dt=> {
                try
                {
                    var ms = new List<DetalleFacturaTrandinaMsg>();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        var item = new DetalleFacturaTrandinaMsg
                        {
                            Cantidad = Convert.ToInt32(dt.Rows[i]["CANTIDAD"]),
                            CodigoArticulo = dt.Rows[i]["CODIGORECURSO"].ToString(),
                            Subtotal = Convert.ToDecimal( dt.Rows[i]["MONTO"])
                        };
                        ms.Add(item);
                    }
                    return ms;
                }
                catch (Exception e)
                {
                    throw ExceptionManager.GetDeepErrorMessage(e, ExceptionManager.eCapa.Core);
                }
            });
        }
        public static string SqlDetalleFacturaTrandinaByIdFactura(string idFactura) {
            return string.Format(@"
                        select 
	                        df.cantidad,
	                        df.CODIGORECURSO,
	                        df.monto
                        from
	                        JBPVW_ORDENFACTURA o inner join
	                        JBPVW_DETALLEFACTURA df on df.idOrden=o.id
                        where o.idFactura={0}
                    ",idFactura);
        }
    }
}
