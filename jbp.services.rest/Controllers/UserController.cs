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
    public class UserController : ApiController
    {
        [HttpPost]
        [Route("api/user/login")]
        public RespAuthMsg Login ([FromBody]LoginMsg me)
        {
            return UserBusiness.GetUser(me);
            
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}