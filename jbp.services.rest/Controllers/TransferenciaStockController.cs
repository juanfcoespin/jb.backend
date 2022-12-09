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
    public class TransferenciaStockController : ApiController
    {
        //esta api utiliza el sistema de balanzas pesaje Espinoza - Paez
        // en el mensaje de entrada debe estar bodegas de orign y desitno asi como los respectivos lotes 
        // en los artículos
        [HttpPost]
        [Route("api/transferenciaStock")]
        public DocSapInsertadoMsg TransferFromBalanzas([FromBody] TsBalanzasMsg me)
        {
            return TransferenciaStockBussiness.TransferFromBalanzas(me);
        }

        [HttpPost]
        [Route("api/tsUbicaciones")]
        public DocSapInsertadoMsg TransferUbicaciones([FromBody] TsBodegaMsg me)
        {
            return TransferenciaStockBussiness.TransferToUbicaciones(me);
        }

        [HttpPost]
        [Route("api/tsFromST")]
        public DocSapInsertadoMsg TransferFromPicking([FromBody] TsFromPickingME me)
        {
            return TransferenciaStockBussiness.SaveFromST(me);
        }

        private SalidaBodegaMsg translate(TsFromPickingME me)
        {
            throw new NotImplementedException();
        }
    }
}