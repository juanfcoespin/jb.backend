using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using jbp.msg;

namespace jbp.business
{
    public class CuentaBusiness
    {
        public MontoCuentaMsg GetMontoCuentaByIdPeriodo_y_cuenta(int idPerido, string cuenta) {
            var ms = new MontoCuentaMsg();
            return ms;
        }
        public static SavedMs SavePlanCuenta(PlanCuentasProcesadoMsg me)
        {
            try
            {
                var cuentaCore = new CuentaCore();
                if (cuentaCore.ExistPlanCuentas(me))
                    cuentaCore.DeletePlanCuentas(me);
                cuentaCore.Insert(me);
            }
            catch (Exception e)
            {
                e = ExceptionManager.GetDeepErrorMessage(e, ExceptionManager.eCapa.Business);
                return new SavedMs { Saved = false, Error = e.Message };
            }
            return new SavedMs { Saved = true };
        }
        public static ListMS<CuentaMsg> GetList()
        {
            try
            {
                return new CuentaCore().GetList();
            }
            catch (Exception e)
            {
                e = ExceptionManager.GetDeepErrorMessage(e, ExceptionManager.eCapa.Business);
                return new ListMS<CuentaMsg> { Error = e.Message };
            }
        }
    }
}
