using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace jbp.services.signalR.Hubs.Contracts
{
    public interface ITypedHubClient
    {
        Task BroadcastMessage(string type, string payload);
    }
}
