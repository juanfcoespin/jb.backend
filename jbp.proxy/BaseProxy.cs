using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComunDelegates;

namespace jbp.proxy
{
    public class BaseProxy
    {
        public event dStringParameter ShowBackgrounMessageEvent;
        public event dStringParameter ShowErrorMessageEvent;
        public void ShowBackgrounMessage(string msg)
        {
            ShowBackgrounMessageEvent?.Invoke(msg);
        }
        public void ClearBackgrounMessage()
        {
            ShowBackgrounMessageEvent?.Invoke(string.Empty);
        }
        public void ShowErrorMessage(string msg) {
            ShowErrorMessageEvent?.Invoke(msg);
        }
    }
}
