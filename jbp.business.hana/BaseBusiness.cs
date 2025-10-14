using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTools.Net;
using TechTools.Rest;

namespace jbp.business.hana
{
   
    public class BaseBusiness
    {
        public enum eMessageType
        {
            Info,
            Success,
            Warning,
            Error
        }
        public class SignalRMsg
        {
            public string message { get; set; }
        }
        public class CommunicationMsg
        {
            public string fecha { get; set; }
            public string userId { get; set; }
            public string message { get; set; }
            public eMessageType tipo { get; set; }
        }
        public static string SendMessageToClient(string clientId, string msg, eMessageType tipo=eMessageType.Info)
        {
            try {
                if (string.IsNullOrEmpty(clientId))
                    return null;
                var rc = new RestCall();
                var url = conf.Default.urlSignalR;
                var me = new CommunicationMsg
                {
                    
                    userId = clientId,
                    fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    message = msg,
                    tipo = tipo
                };
                var resp = (SignalRMsg)rc.SendPostOrPut(url, typeof(SignalRMsg), me, typeof(CommunicationMsg), RestCall.eRestMethod.POST);
                return resp.message;
            }
            catch(Exception e) {
                return e.Message;
            }
        }
        public bool EnviarPorCorreo(string to, string titulo, string msg, ref string error, List<string> filePaths=null)
        {
            return MailUtils.Send(to,titulo,msg,ref error,filePaths);
        }
    }
}
