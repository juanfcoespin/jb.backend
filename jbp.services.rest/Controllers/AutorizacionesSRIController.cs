using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using DxSriDll;
using TechTools.Rest;
using TechTools.DelegatesAndEnums;

namespace jbp.services.rest.Controllers
{
    public class AutorizacionesSRIController : ApiController
    {
        [HttpGet]
        [Route("api/autorizacionSRI/enviarRetenciones/{mesesRetencion}")]
        public void EnviarRetenciones(string mesesRetencion)
        {
            Retenciones.eNotificarMsg += Retenciones_eNotificarMsg;
            Retenciones.EnviarRetenciones(mesesRetencion);
            Retenciones.eNotificarMsg -= Retenciones_eNotificarMsg;
        }

        private void Retenciones_eNotificarMsg(string msg)
        {
            //se envia el mensaje consumiendo un servicio de signal R
            var rc = new RestCall();
            var url = string.Format("{0}/{1}",Properties.Settings.Default.RetencionesSendStatusWS_Url,msg);
            rc.SendGetAsync(url, typeof(void));
        }
    }
}
