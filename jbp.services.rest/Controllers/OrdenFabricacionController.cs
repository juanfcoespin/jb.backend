using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using jbp.business.hana;
using jbp.msg.sap;
using jbp.msg;

namespace jbp.services.rest.Controllers
{
    public class OrdenFabricacionController : ApiController
    {
        //se quita esta api porque la cantidad pesada se inyecta en la trasferencia de stock
        /*[HttpPost]
        [Route("api/setCantPesadaComponenteOF")]
        public BoolMs TransferFromBalanzas([FromBody] CantPesadaComponenteOF me)
        {
            return BodegaBusiness.SetCantPesadaComponenteOF(me);
        }*/
        [HttpGet]
        [Route("api/of/getOfLiberadasPesaje")]
        public List<OrdenFabricacionLiberadaPesajeMsg> GetOfLiberadasPesaje()
        {
            return OrdenFabricacionBusiness.GetOfLiberadasPesaje();
        }
        [HttpGet]
        [Route("api/of/getOfLiberadasPesaje/{codPT}/{codInsumo}")]
        public List<OrdenFabricacionLiberadaPesajeMsg> GetOfLiberadasPesaje(string codPT,  string codInsumo)
        {
            return OrdenFabricacionBusiness.GetOfLiberadasPesaje(codPT, codInsumo);
        }

        [HttpGet]
        [Route("api/of/getComponentesOf/{docNum}")]
        public OFMasComponentesMsg GetComponentesOfByDocNum(int docNum)
        {
            return OrdenFabricacionBusiness.GetComponentesAPesarOfByDocNum(docNum);
        }
        
        [HttpGet]
        [Route("api/of/getComponentesOf/{docNum}/{codInsumo}")]
        public OFMasComponentesMsg GetComponentesOfByDocNum(int docNum, string codInsumo)
        {
            return OrdenFabricacionBusiness.GetComponentesAPesarOfByDocNum(docNum, codInsumo);
        }
    }
}
