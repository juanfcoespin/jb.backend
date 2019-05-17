using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComunDelegates;

namespace jbp.business.contracts
{
    interface INotificationLog
    {
        event dLogNotification LogNotificationEvent;
    }
}
