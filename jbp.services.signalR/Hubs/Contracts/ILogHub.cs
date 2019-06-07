using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TechTools.Msg;

namespace jbp.services.signalR.Hubs.Contracts
{
    public interface ILogHub
    {
        Task PushLog(LogMsg me);
    }
}
