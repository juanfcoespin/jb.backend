using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using jbp.msg.sap;
using jbp.business.hana;
using System.Threading;


namespace jbp.services.rest.Controllers
{
    public class SolicitudTransferenciaController : ApiController
    {

        [HttpGet]
        [Route("api/st/getST_OF_Liberadas")]
        public List<ST_OF_LiberadasMsg> GetST_OF_Liberadas()
        {
            return SolicitudTransferenciaBusiness.GetST_OF_Liberadas();
        }

        [HttpGet]
        [Route("api/st/GetComponetesConLotesById/{id}")]
        public List<ST_ComponentesMsg> GetComponetesConLotesById(string id)
        {
            return SolicitudTransferenciaBusiness.GetComponetesConLotesById(Convert.ToInt32(id));
        }
    }
}