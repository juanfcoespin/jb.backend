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
        public void EnviarPorCorreo(string to, string titulo, string msg, List<string> filePaths=null)
        {
            MailUtils.Send(
                to,
                titulo,
                msg,
                filePaths
            );
        }

        public void testCorreo(string msg)
        {
            try
            {
                EnviarPorCorreo("jespin@jbp.com.ec", "Msg prueba", msg);
            }
            catch (Exception e)
            {
                msg = e.Message;
                throw;
            }
            
        }
    }
}
