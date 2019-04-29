using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbp.msg
{
    public enum eTipoTercero
    {
        TRANDINA,
        PROMOTICK,
        NotDefined
    }
    public enum eTipoLog
    {
        info,
        error,
        warning
    }
    public enum eTipoDocFuente
    {
        factura=0,
        facturaServicios=2,
        NotasCredito=3
    }
}
