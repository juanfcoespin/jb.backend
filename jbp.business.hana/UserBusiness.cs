using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jbp.msg;
using System.Runtime.InteropServices;
using TechTools.Exceptions;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices;
using TechTools.Core.Hana;
using System.Data;



namespace jbp.business.hana
{
    public class UserBusiness
    {
        // para autenticarse con active directory
        [DllImport("advapi32.dll")]
        public static extern bool LogonUser(string name, string domain, string pss, int logType, int logpv, ref IntPtr pht);


        public static bool LogOnAD(string user, string pwd)
        {
            var th = IntPtr.Zero;
            return LogonUser(user, GetDomainName(), pwd, 2, 0, ref th);
        }

        public static RespAuthMsg newUserAppMovil(string userMail)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Extrae el nombre del dominio del equipo
        /// </summary>
        /// <param name="connectedServer">Ej svr1.jbp.com.ec</param>
        /// <returns>jbp.com.ec</returns>
        private static string GetDomainName()
        {
            var domain = new PrincipalContext(ContextType.Domain);
            var ms = string.Empty;
            var connectedServer = domain.ConnectedServer;
            var vect = connectedServer.Split(new char[] { '.' });
            var hostName = vect[0] + ".";
            ms = connectedServer.Replace(hostName, null);
            return ms;
        }

        public static object GetUserDetails(string userName)
        {
            using (PrincipalContext context = new PrincipalContext(ContextType.Domain))
            {
                UserPrincipal user = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, userName);
                if (user != null)
                {
                    return new {
                        userDisplayName = user.DisplayName,
                        userMail = user.EmailAddress
                    };
                }
                return null;
            }
        }


        public static RespAuthMsg GetUser(LoginMsg me)
        {
            var ms = new RespAuthMsg();
            try
            {
                if (LogOnAD(me.User,me.Pwd)) {
                    var domain = new PrincipalContext(ContextType.Domain);
                    var user = UserPrincipal.FindByIdentity(domain, me.User);
                    ms.UserName = me.User;
                    ms.Nombre = user.DisplayName ?? user.Name;
                    ms.correo= user.EmailAddress;
                    
                    foreach (var group in user.GetGroups()) {
                        ms.GruposDirectorioActivo.Add(group.Name);
                    }
                    //ms.Perfiles = GetPerfilesByUserName(me.User);
                    ms.ModulosAcceso = GetModulosAcceso(ms);
                    ms.IdVendor = GetIdVendorByUserName(me.User);
                    LogLogin(ms, me.AppName);
                }
            }
            catch (Exception e)
            {
                e=ExceptionManager.GetDeepErrorMessage(e, ExceptionManager.eCapa.Business);
                ms.Error = e.Message;
            }
            return ms;
        }

        private static void LogLogin(RespAuthMsg me, string AppName)
        {
            var logMe = new LogMsg
            {
                UserName = me.Nombre,
                AppName = AppName,
                Obs = "Login"
            };
            Log(logMe);
        }
        public static string Log(LogMsg me)
        {
            try
            {
                var sql = string.Format(@"
                insert into JB_LOG(USER, APP_NAME, FECHA, OBS)
                VALUES('{0}', '{1}', CURRENT_TIMESTAMP, '{2}')
                ", me.UserName, me.AppName, me.Obs);
                new BaseCore().Execute(sql);
                return "ok";
            }
            catch(Exception e) {
                return e.Message;
            }
        }

        public static List<string> GetModulosAcceso()
        {
            var ms = new List<string>();
            ms = TechTools.Utils.ObjectUtils.GetMemberNames(typeof(ModulosAccesoMS));
            return ms;
        }
        
        public static ModulosAccesoMS GetModulosAcceso(RespAuthMsg me) {
            var ms = new ModulosAccesoMS();
            // Por grupo de AD
            if (me.GruposDirectorioActivo!=null)
            {
                me.GruposDirectorioActivo.ForEach(grupo => {
                    if (grupo.ToLower() == "tics") //administrador
                    {
                        ms.Bodega = true;
                        ms.Ventas = true;
                        ms.FarmacoVigilancia = true;
                        ms.Dashboards = true;
                        ms.ControlCalidad = true;
                    }
                    if (grupo.ToLower() == "ventas"){
                        ms.Ventas = true;
                    }
                    if (grupo.ToLower() == "dashboards")
                    {
                        ms.Dashboards = true;
                    }
                    if (grupo.ToLower() == "bodega"){
                        ms.Bodega = true;
                    }
                    if (grupo.ToLower() == "controlcalidad")
                    {
                        ms.ControlCalidad = true;
                    }
                    if (grupo.ToLower() == "asuntosregulatorios")
                    {
                        ms.FarmacoVigilancia = true;
                    }
                    if (grupo.ToLower() == "visitadoresmedicos")
                    {
                        ms.VisitadoresMedicosFarmacias = true;
                    }
                });
            }
            return ms;
        }
        private static int GetIdVendorByUserName(string userName)
        {
            var ms = 0;
            var sql = string.Format(@"
                select ""CodVendedor""
                from ""JbpVw_Vendedores""
                where ""Email"" = '{0}@jbp.com.ec'
            ", userName);
            ms = new BaseCore().GetIntScalarByQuery(sql);
            return ms;
        }

    }
}
