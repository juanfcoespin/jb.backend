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
        [HttpGet]
        [Route("api/user/GetUserDetails/{userName}")]
        public object GetUserDetails(string userName)
        {
            return UserBusiness.GetUserDetails(userName);
        }

        [HttpGet]
        [Route("api/user/newUserAppMovil/{userMail}")]
        public RespAuthMsg newUserAppMovil(string userMail)
        {
            return UserBusiness.newUserAppMovil(userMail);
        }

        [HttpPost]
        [Route("api/user/login")]
        public RespAuthMsg Login ([FromBody]LoginMsg me)
        {
            return UserBusiness.GetUser(me);
        }
        [HttpGet]
        [Route("api/user/getModulosAcceso")]
        public List<string> GetModulosAcceso() { 
            return UserBusiness.GetModulosAcceso();
        }

        [HttpPost]
        [Route("api/user/log")]
        public string Log([FromBody] LogMsg me)
        {
            return UserBusiness.Log(me);
        }

        
    }
}