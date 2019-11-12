using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTools.DelegatesAndEnums;

namespace jbp.business
{
    public class BaseBusiness
    {
        public event dStringParameter eNotifyMsg;

        public void NotifyMsg(string msg) {
            eNotifyMsg?.Invoke(msg);
        }

    }
}
