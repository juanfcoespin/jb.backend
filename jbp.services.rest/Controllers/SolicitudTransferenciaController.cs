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
        [HttpPost]
        [Route("api/st/getST_OF_Liberadas")]
        public object GetST_OF_Liberadas([FromBody] FiltroPickingProdME me)
        {
            return SolicitudTransferenciaBusiness.GetST_OF_Liberadas(me);
        }
        [HttpPost]
        [Route("api/st/save")]
        public DocSapInsertadoMsg Save([FromBody] StMsg me)
        {
            return SolicitudTransferenciaBusiness.Save(me);
        }

        [HttpGet]
        [Route("api/st/GetComponetesConLotesById/{id}")]
        public List<ST_ComponentesMsg> GetComponetesConLotesById(string id)
        {
            return SolicitudTransferenciaBusiness.GetComponetesConLotesById(Convert.ToInt32(id));
        }
    }
}