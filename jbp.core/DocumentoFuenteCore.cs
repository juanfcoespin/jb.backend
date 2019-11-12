using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbp.core
{
    public class DocumentoFuenteCore
    {
        public static string SqlFechaByNumFacturaAndRuc(string numFactura, string ruc)
        {
            return string.Format(@"
                SELECT
                 to_char(DOCDATE,'dd/mm/yyyy') AS FECHAFACT
                FROM FGSRCDOC
                WHERE SRCDOC = '{0}' AND SRCDOCCREATOR = '{1}'
            ", numFactura,ruc);
        }
    }
}
