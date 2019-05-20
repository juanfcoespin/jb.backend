using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DelegatesAndEnums;

namespace jbp.business.contracts
{
    interface INotificationLog
    {
        event dLogNotification LogNotificationEvent;
    }
}
