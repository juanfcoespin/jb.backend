using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbp.core
{
    public class ParticipantesPuntosCore:BaseCore
    {
        public bool ExisteParticipanteByRuc(string ruc) {
            var sql = string.Format(@"
                select 
                    count(*)
                from 
                    gms.TBL_CLIENTES_PUNTOSJB
                where ruc='{0}' or ruc_prin='{0}'
            ", ruc);
            return GetIntScalarByQuery(sql) > 0;
        }
    }
}
