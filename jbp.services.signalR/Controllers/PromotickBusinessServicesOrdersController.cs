using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.SignalR;
using jbp.services.signalR.Hubs;
using jbp.services.signalR.Hubs.Contracts;
using TechTools.Msg;

namespace jbp.services.signalR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromotickBusinessServicesOrdersController : ControllerBase
    {
        private IHubContext<CheckOrdersToPromotickBusinessService, IBusinessServicesHub> HubContext;
        public PromotickBusinessServicesOrdersController(IHubContext<CheckOrdersToPromotickBusinessService, IBusinessServicesHub> hubContext)
        {
            this.HubContext = hubContext;
        }
        [HttpPost("log")]
        public void Log([FromBody]LogMsg me)
        {
           this.HubContext.Clients.All.PushLog(me);
        }
        [HttpGet("start")]
        public void Start()
        {
            this.HubContext.Clients.All.Start();
        }
        [HttpGet("stop")]
        public void Stop()
        {
            this.HubContext.Clients.All.Stop();
        }
        [HttpGet("status")]
        public void Status()
        {
            this.HubContext.Clients.All.Status();
        }
        [HttpGet("checkIsRunning")]
        public void CheckIsRunning()
        {
            this.HubContext.Clients.All.CheckIsRunning();
        }
        [HttpGet("isRunningResponse/{isRunning}")]
        public void IsRunningResponse(bool isRunning)
        {
            this.HubContext.Clients.All.IsRunningResponse(isRunning);
        }
        
    }
}