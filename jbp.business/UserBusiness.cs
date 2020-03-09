using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jbp.msg;
using System.Runtime.InteropServices;
using TechTools.Exceptions;
using System.DirectoryServices.AccountManagement;

namespace jbp.business
{
    public class UserBusiness
    {
        // para autenticarse con active directory
        [DllImport("advapi32.dll")]
        public static extern bool LogonUser(string name, string domain, string pss, int logType, int logpv, ref IntPtr pht);
        
        
        public static RespAuthMsg GetUser(LoginMsg me)
        {
            var ms = new RespAuthMsg();
            try
            {
                if (LogOnAD(me.User,me.Pwd)) {
                    var domain = new PrincipalContext(ContextType.Domain);
                    var user = UserPrincipal.FindByIdentity(domain, me.User);
                    ms.Nombre = user.DisplayName ?? user.Name;
                    foreach (var group in user.GetGroups()) {
                        ms.Perfiles.Add(group.Name);
                    }
                }
            }
            catch (Exception e)
            {
                e=ExceptionManager.GetDeepErrorMessage(e, ExceptionManager.eCapa.Business);
                ms.Error = e.Message;
            }
            return ms;
        }
        public static bool LogOnAD(string user, string pwd) {
            var domain = new PrincipalContext(ContextType.Domain);
            var th = IntPtr.Zero;
            return LogonUser(user, GetDomainName(domain.ConnectedServer), pwd, 2, 0, ref th);
        }

        /// <summary>
        /// Extrae el nombre del dominio del equipo
        /// </summary>
        /// <param name="connectedServer">Ej svr1.jbp.com.ec</param>
        /// <returns>jbp.com.ec</returns>
        private static string GetDomainName(string connectedServer)
        {
            var ms = string.Empty;
            var vect = connectedServer.Split(new char[] {'.'});
            var hostName = vect[0] + ".";
            ms = connectedServer.Replace(hostName, null);
            return ms;
        }
    }
}
