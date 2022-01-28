using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jbp.msg;
using System.Runtime.InteropServices;
using TechTools.Exceptions;
using System.DirectoryServices.AccountManagement;
using TechTools.Core.Hana;
using System.Data;

namespace jbp.business.hana
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
                    ms.correo= user.EmailAddress;
                    foreach (var group in user.GetGroups()) {
                        ms.GruposDirectorioActivo.Add(group.Name);
                    }
                    ms.Perfiles = GetPerfilesByUserName(me.User);
                }
            }
            catch (Exception e)
            {
                e=ExceptionManager.GetDeepErrorMessage(e, ExceptionManager.eCapa.Business);
                ms.Error = e.Message;
            }
            return ms;
        }
        public static RespAuthVendorMsg GetVendorUser(LoginMsg me)
        {
            var ms = new RespAuthVendorMsg();
            var user = GetUser(me);
            if (user.Perfiles.Count > 0)
            {
                ms.Nombre = user.Nombre;
                ms.Perfiles = user.Perfiles;
                ms.GruposDirectorioActivo = user.GruposDirectorioActivo;
                if (user.IsVendor())
                    ms.IdVendor = GetIdVendorByUserName(me.User);
            }
            return ms;
        }

        private static int GetIdVendorByUserName(string userName)
        {
            var ms = 0;
            var userEmail = GetUserEmailFromUserName(userName);
            var sql = string.Format(@"
                select ""CodVendedor""
                from ""JbpVw_Vendedores""
                where ""Email"" = '{0}'
            ", userEmail);
            ms = new BaseCore().GetIntScalarByQuery(sql);
            return ms;
        }

        private static List<string> GetPerfilesByUserName(string userName)
        {
            var profiles = new List<string>();
            var userEmail = GetUserEmailFromUserName(userName);
            var sql = string.Format(@"
                SELECT 
                 T1.PROFILE
                FROM JBP_USER_PROFILE T0 INNER JOIN
                 JBP_PROFILE T1 ON T1.ID=T0.ID_PROFILE INNER JOIN
                 JBP_USER T2 ON T2.ID=T0.ID_USER
                WHERE
                 T2.MAIL='{0}'
            ", userEmail);
            var dt = new BaseCore().GetDataTableByQuery(sql);
            foreach(DataRow dr in dt.Rows)
            {
                profiles.Add(dr["PROFILE"].ToString());
            }
            return profiles;
        }

        private static object GetUserEmailFromUserName(string userName)
        {
            var domain = "jbp.com.ec";
            var ms = userName.Replace("jamesbrownpharma.com", domain);
            if (!userName.Contains(domain))
            {
                ms = string.Format("{0}@{1}", userName, domain);
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
