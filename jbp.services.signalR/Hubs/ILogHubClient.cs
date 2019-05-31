using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TechTools.Msg;

namespace jbp.services.signalR.Hubs
{
    public interface ILogHubClient
    {
        Task PushLog(LogMsg me);
    }
}
