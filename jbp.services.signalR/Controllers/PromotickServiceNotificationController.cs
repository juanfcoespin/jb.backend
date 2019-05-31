using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.SignalR;
using jbp.services.signalR.Hubs;
using TechTools.Msg;

namespace jbp.services.signalR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromotickServiceNotificationController : ControllerBase
    {
        private IHubContext<LogPromotickServiceHub, ILogHubClient> hubContext;
        public PromotickServiceNotificationController(IHubContext<LogPromotickServiceHub, ILogHubClient> hubContext) {
            this.hubContext = hubContext;
        }
        [HttpPost]
        public string Post([FromBody]LogMsg me)
        {
            try
            {
                this.hubContext.Clients.All.PushLog(me);
                return "ok";
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }
    }
}