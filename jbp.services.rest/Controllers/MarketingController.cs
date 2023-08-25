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
    public class MarketingController : ApiController
    {

        [HttpGet]
        [Route("api/marketing/getDasboards")]
        public DashBoardsMsg getDasboards()
        {
            return MarketingBusiness.GetDasboards();
        }

        [HttpGet]
        [Route("api/marketing/deleteDasboard/{id}")]
        public string deleteDasboard(int id)
        {
            return MarketingBusiness.deleteDasboard(id);
        }

        [HttpPost]
        [Route("api/marketing/saveDashboard")]
        public Dash setDashboard([FromBody] Dash me)
        {
            return MarketingBusiness.SaveDashboard(me);
        }

    }
}