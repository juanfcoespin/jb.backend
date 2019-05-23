using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using jbp.business.services;
using TechTools.DelegatesAndEnums;
using TechTools.Utils;
using TechTools.Msg;
using jbp.msg;

namespace jbp.services.rest.Controllers
{
    public class FacturaPromotickController : ApiController
    {
        private static CheckFacturasToSendPtkBusinessService servicePtk;
        public FacturaPromotickController() {
            if (servicePtk == null)
            {
                servicePtk = new CheckFacturasToSendPtkBusinessService();
                servicePtk.LogNotificationEvent += (tipo, msg) => NotificarEvento(tipo, msg);
            }
        }
        
        private void NotificarEvento(eTypeLog tipo, string msg)
        {
            msg = string.Format("{0}: {1}", DateTime.Now.ToString("yyyy-mm-dd HH:mm:ss"), msg);
            var url = "http://localhost:5000/api/message";
            var rc = new RestCall();
            var me = new TestMsg { Type = tipo.ToString(), Payload = msg };
            rc.SendPostOrPutAsync(url, typeof(string), me, typeof(TestMsg), RestCall.eTypeSend.POST);
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
            return LogUtils.GetLogsByDate(me);
        }
        [HttpGet]
        [Route("api/facturaPromotick/isRunning")]
        public bool IsRunning()
        {
            return servicePtk.IsRunning();
        }
    }
}
