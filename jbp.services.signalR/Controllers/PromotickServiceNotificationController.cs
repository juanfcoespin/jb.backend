using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.SignalR;
using jbp.services.signalR.Hubs;
using TechTools.Msg;
using jbp.services.signalR.Hubs.Contracts;

namespace jbp.services.signalR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromotickServiceNotificationController : ControllerBase
    {
        private IHubContext<LogHub, ILogHub> HubContext;
        public PromotickServiceNotificationController(IHubContext<LogHub, ILogHub> hubContext) {
            this.HubContext = hubContext;
        }
        [HttpPost]
        public string Post([FromBody]LogMsg me)
        {
            try
            {
                this.HubContext.Clients.All.PushLog(me);
                return "ok";
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }
    }
}