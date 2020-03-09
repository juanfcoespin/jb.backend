using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using jbp.msg;

namespace jbp.services.signalR.Hubs.Contracts
{
    public interface IStatusManager
    {
        Task SendMessage(StatusMsg me);
        Task RequestMessage();
    }
}
