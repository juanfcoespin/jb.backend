using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTools.DelegatesAndEnums;

namespace jbp.business.oracle9i.contracts
{
    public interface INotificationLog
    {
        event dLogNotification LogNotificationEvent;
    }
}
