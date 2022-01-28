using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTools.Rest;

namespace jbp.business.hana
{
    public class BaseWSPtk:BaseBusiness
    {
        public RestCall.AuthenticationMe credencialesWsPromotick = null;
        public BaseWSPtk()
        {
            this.credencialesWsPromotick = new RestCall.AuthenticationMe
            {
                User = conf.Default.ptkWsUser,
                Pwd = conf.Default.ptkWsPwd,
                AuthType = RestCall.eAuthType.Basic
            };
        }
        public void EnviarPorCorreo(string titulo, string msg)
        {
            this.EnviarPorCorreo(conf.Default.CorreosNotificacionesPromotick, titulo, msg);
            
        }
    }
}
