using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using jbp.msg;
using jbp.core;
using TechTools.Utils;

namespace jbp.business
{
    public class PeriodoBusiness
    {
        public static PeriodoMsg GetById(int id) {
            try
            {
                return new PeriodoCore().GetById(id);
            }
            catch (Exception e)
            {
                e = ExceptionManager.GetDeepErrorMessage(e, ExceptionManager.eCapa.Business);
                return new PeriodoMsg { Error = e.Message };
            }
        }
        public static ListMS<ItemCombo> GetList() {
            try
            {
                return new PeriodoCore().GetList();
            }
            catch (Exception e)
            {
                e = ExceptionManager.GetDeepErrorMessage(e, ExceptionManager.eCapa.Business);
                return new ListMS<ItemCombo> { Error = e.Message };
            }
        }
    }
}
