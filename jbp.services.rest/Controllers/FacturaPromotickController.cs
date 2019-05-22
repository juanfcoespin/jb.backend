using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using jbp.business.services;


namespace jbp.services.rest.Controllers
{
    public class FacturaPromotickController : ApiController
    {
        private static CheckFacturasToSendPtkBusinessService servicePtk;
        private static List<string> logs;
        public FacturaPromotickController() {
            if (logs == null)
                logs = new List<string>();
            if (servicePtk == null)
            {
                servicePtk = new CheckFacturasToSendPtkBusinessService();
                servicePtk.LogNotificationEvent += (tipo, msg) =>
                {
                    logs.Add(string.Format("{0}: {1}", tipo, msg));
                };
            }
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
        [Route("api/facturaPromotick/logs")]
        public List<string> Logs()
        {
            return logs;
        }
        [HttpGet]
        [Route("api/facturaPromotick/isRunning")]
        public bool IsRunning()
        {
            return servicePtk.IsRunning();
        }
    }
}
