using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using jbp.msg;
using jbp.core.oracle9i;
using TechTools.Exceptions;
using System.Reactive.Linq;

using System.Threading;
using System.Reactive.Disposables;

namespace jbp.business.oracle9i
{
    public class CuentaBusiness
    {
        public MontoCuentaMsg GetMontoCuentaByIdPeriodo_y_cuenta(int idPerido, string cuenta)
        {
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
        public static ProcessCheckedMsg SetMontosPorPeriodo(int idPeriodo)
        {
            try
            {
                var ms = new ProcessCheckedMsg();
                ms.Id = CommonBusiness.ListProcessChecked.Count();
                ms.Total = 50;
                CommonBusiness.ListProcessChecked.Add(ms);

                //se corre el proceso de manera asíncrona
                var obs = SetMontosPorPeriodoAsync();
                //obs.Subscribe(o =>
                //    ms.Current = o
                //);
                return ms;
            }
            catch (Exception e)
            {
                e = ExceptionManager.GetDeepErrorMessage(e, ExceptionManager.eCapa.Business);
                return new ProcessCheckedMsg { Error = e.Message };
            }
        }
        public static IObservable<int> SetMontosPorPeriodoAsync()
        {
            return Observable.Create<int>(o =>
            {
                // internally creates a new CancellationTokenSource
                var cancel = new CancellationDisposable();

                for (int i = 0; i <= 50; i++)
                {
                    Thread.Sleep(100);
                    o.OnNext(i);
                }
                o.OnCompleted();
                return cancel;
            });
        }

    }
}
