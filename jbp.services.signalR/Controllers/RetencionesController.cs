using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.SignalR;
using jbp.services.signalR.Hubs;
using jbp.services.signalR.Hubs.Contracts;
using jbp.msg;

namespace jbp.services.signalR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RetencionesController : ControllerBase
    {
        private IHubContext<StatusManager, IStatusManager> HubContext;

        public RetencionesController(IHubContext<StatusManager, IStatusManager> hubContext)
        {
            this.HubContext = hubContext;
        }

        [HttpGet("sendMessage/{msg}")]
        public void SendMessage(string msg)
        {
            this.HubContext.Clients.All.SendMessage(new StatusMsg
            {
                Date = DateTime.Now.ToString(),
                Msg = msg
            });
            
        }
        [HttpGet("requestMessage")]
        public void RequestMessage()
        {
            this.HubContext.Clients.All.RequestMessage();
        }
    }
}