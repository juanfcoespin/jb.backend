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
        [Route("api/facturaPromotick/status")]
        public string Status()
        {
            return servicePtk.GetStatus();
        }
        [HttpGet]
        [Route("api/facturaPromotick/getLogsByDate/{me}")]
        public List<LogMsg> GetLogsByDate(string me)
        {
            return LogBusiness.GetLogsByDate(me);
        }
        [HttpGet]
        [Route("api/facturaPromotick/GetTodayLogs")]
        public List<LogMsg> GetTodayLogs()
        {
            var today = DateTime.Now.ToString("yyyy-MM-dd");
            return LogBusiness.GetLogsByDate(today);
        }
        [HttpGet]
        [Route("api/facturaPromotick/isRunning")]
        public bool IsRunning()
        {
            return servicePtk.IsRunning();
        }
    }
}
