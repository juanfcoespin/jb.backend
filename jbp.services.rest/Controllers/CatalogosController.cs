using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using jbp.msg;
using jbp.business.hana;

namespace jbp.services.rest.Controllers
{
    public class CatalogosController : ApiController
    {
        [HttpGet]
        [Route("api/catalogos/getbancosecuador")]
        public List<ItemCombo> GetBancosEcuador()
        {
            return BancoBusiness.GetBancosEcuador();
        }

        [HttpGet]
        [Route("api/catalogos/getCatalogoPesaje")]
        public CatalogoPesajeMsg GetCatalogoPesaje()
        {
            return jbp.business.hana.CatalogoBusiness.GetCatalogoPesaje();
        }


    }
}
