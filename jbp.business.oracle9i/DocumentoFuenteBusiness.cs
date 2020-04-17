using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTools.Core.Oracle9i;
using jbp.core.oracle9i;

namespace jbp.business.oracle9i
{
    public class DocumentoFuenteBusiness
    {
  

        public static string GetFechaByNumFacturaAndRuc(string numFactura, string ruc)
        {
            return new BaseCore().GetScalarByQuery(
                DocumentoFuenteCore.SqlFechaByNumFacturaAndRuc(numFactura,ruc)
            );
        }
    }
}
