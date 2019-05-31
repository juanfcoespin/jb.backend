using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace jbp.services.signalR.Hubs
{
    public class LogPromotickServiceHub : Hub<ILogHubClient>
    {
    }
}
