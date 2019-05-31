using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using jbp.msg;
using jbp.business;

namespace jbp.services.rest.Controllers
{
    public class FacturaController : ApiController
    {
        [HttpGet]
        [Route("api/factura/getListToSendTrandina")]
        public List<FacturaTrandinaMsg> GetFacturasToSendTrandina()
        {
            var ms = FacturaBusiness.GetFacturasToSendTrandina();
            return ms;
        }
        [HttpPut]
        [Route("api/factura/registrarFacturasEnvioTerceros")]
        public bool RegistrarFacturaEnvioTerceros([FromBody]List<RegistroFacturaTercerosMsg> me)
        {
            var ms = FacturaBusiness.RegistrarFacturaEnvioTerceros(me);
            return ms;
        }
        [HttpPost]
        [Route("api/factura/registrarFacturasJB")]
        public List<ParametroSalidaPtkMsg> RegistrarFacturasJB([FromBody]List<FacturaPromotickMsg> me)
        {
            //se hace un mock
            var listMs = new List<ParametroSalidaPtkMsg>();
            me.ForEach(factura => {
                var ms = new ParametroSalidaPtkMsg();
                ms.numFactura = factura.numFactura;
                ms.codigo = GetRamdomCodigo();
                listMs.Add(ms);
            });
            return listMs;
        }
        private static readonly Random random = new Random();
        private static readonly object syncLock = new object();
        private static int GetRamdomCodigo()
        {
            lock (syncLock)// otherwise get the same random number
            {
                var codigos = new List<int>() { 1, -100, -150, -200, -500 };
                int index = random.Next(codigos.Count);
                return codigos[index];
            }
        }

        [HttpPut]
        [Route("api/factura/desregistrarFacturasEnvioTerceros")]
        public bool DesregistrarFacturaEnvioTerceros([FromBody] List<FacturaTrandinaMsg> me)
        {
            var ms = FacturaBusiness.DesregistrarFacturaEnvioTerceros(me);
            return ms;
        }
    }
}
