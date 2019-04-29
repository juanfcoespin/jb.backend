using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using utilities;
using jbp.core;

namespace jbp.business
{
    public class DetalleFacturaBusiness
    {
        public static DataTable GetDetalleFacturaByIdFactura(string idFactura) {
            try
            {
                var ms = new DataTable();
                var sql = DetalleFacturaCore.SqlDetalleFacturaByIdFactura(idFactura);
                ms = new BaseCore().GetDataTableByQuery(sql);
                return ms;
            }
            catch (Exception e)
            {
                throw ExceptionManager.GetDeepErrorMessage(e,ExceptionManager.eCapa.Business);
            }
        }
    }
}
