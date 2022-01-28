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
    
    public class TransportistaController : ApiController
    {
        
        public List<ItemCombo> get()
        {
            return TransportistaBusiness.GetTransportistas();
        }

    }
}
