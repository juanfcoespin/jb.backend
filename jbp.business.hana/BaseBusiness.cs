using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTools.Net;

namespace jbp.business.hana
{
    public class BaseBusiness
    {
        public void EnviarPorCorreo(string titulo, string msg)
        {
            MailUtils.Send(
                conf.Default.CorreosNotificacionesPromotick,
                titulo,
                msg
            );
        }
    }
}
