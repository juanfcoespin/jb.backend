using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;
using jbp.services.signalR.Hubs.Contracts;

namespace jbp.services.signalR.Hubs
{
    public class LogHub : Hub<ILogHub>
    {
    }
}
