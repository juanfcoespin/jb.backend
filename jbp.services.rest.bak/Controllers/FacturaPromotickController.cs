using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using jbp.business;
using jbp.business.services;
using TechTools.Msg;
using jbp.msg;

namespace jbp.services.rest.Controllers
{
    public class FacturaPromotickController : ApiController
    {
        private static CheckFacturasToSendPtkBusinessService servicePtk;
        public FacturaPromotickController() {
            if (servicePtk == null)
                servicePtk = new CheckFacturasToSendPtkBusinessService();
        }
        [HttpGet]
        [Route("api/facturaPromotick/start")]
        public string Start()
        {
            if (!servicePtk.IsRunning())
                servicePtk.Start(60);
            return "starting";
        }
        [HttpGet]
        [Route("api/facturaPromotick/stop")]
        public string Stop()
        {
            if (servicePtk.IsRunning())
                servicePtk.Stop();
            return "stoping";
        }
        [HttpGet]
        [Route("api/facturaPromotick/status")]
        public string Status()
        {
            return servicePtk.GetStatus();
        }
        [HttpGet]
        [Route("api/facturaPromotick/logByDate/{me}")]
        public List<LogMsg> LogByDate(string me)
        {
            return LogBusiness.GetLogByDate(me);
        }
        [HttpGet]
        [Route("api/facturaPromotick/isRunning")]
        public bool IsRunning()
        {
            return servicePtk.IsRunning();
        }
    }
}
