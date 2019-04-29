using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using jbp.msg;
using jbp.core;
using utilities;

namespace jbp.business
{
    public class PeriodoBusiness
    {
        public static PeriodoMsg GetByNombre(string me) {
            try
            {
                return PeridoCore.GetByNombre(me);
            }
            catch (Exception e)
            {
                e = ExceptionManager.GetDeepErrorMessage(e, ExceptionManager.eCapa.Business);
                return new PeriodoMsg { Error = e.Message };
            }
        }
    }
}
