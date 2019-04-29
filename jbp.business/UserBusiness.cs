using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jbp.msg;

namespace jbp.business
{
    public class UserBusiness
    {
        public static RespAuthMsg GetUser(LoginMsg me)
        {
            var listUsers = GetListUser();
            var user = listUsers.Find(p => p.User == me.User && p.Pwd == me.Pwd);
            if (user!=null)
            {
                return new RespAuthMsg
                {
                    Nombre = user.Nombre,
                    Perfiles = user.Perfiles
                };
            }
            return null;
        }
        private static List<UserMsg> GetListUser() {
            var listUsers = new List<UserMsg>();
            listUsers.Add(
                new UserMsg
                {
                    Nombre = "Juan Francisco Espín",
                    User = "juan",
                    Pwd = "2816Jfen",
                    Perfiles = new List<string>() { "Administrador" }
                }
            );
            listUsers.Add(
                new UserMsg
                {
                    Nombre = "Gardenia Landacay",
                    User = "gardenia",
                    Pwd = "gardenia",
                    Perfiles = new List<string>() { "Ventas" }
                }
            );
            return listUsers;
        }
    }
}
