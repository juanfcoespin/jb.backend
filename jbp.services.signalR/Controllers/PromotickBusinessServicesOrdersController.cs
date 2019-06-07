using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.SignalR;
using jbp.services.signalR.Hubs;
using jbp.services.signalR.Hubs.Contracts;

namespace jbp.services.signalR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromotickBusinessServicesOrdersController : ControllerBase
    {
        private IHubContext<SendFacturasNcPromotickHub, IBusinessServicesHub> HubContext;
        public PromotickBusinessServicesOrdersController(IHubContext<SendFacturasNcPromotickHub, IBusinessServicesHub> hubContext)
        {
            this.HubContext = hubContext;
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
    }
}