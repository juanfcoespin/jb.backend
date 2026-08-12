using jbp.msg;
using jbp.msg.sap;
using System;
using System.Collections.Generic;
using System.Data;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTools.Core.Hana;


namespace jbp.business.hana
{
    public class MarketingBusiness
    {
        public static DashBoardsMsg GetDasboards(string userName)
        {
            try
            {
                var grupos = new Dictionary<string, string>();

                using (var domain = new PrincipalContext(ContextType.Domain))
                {
                    using (var user = UserPrincipal.FindByIdentity(domain, userName))
                    {
                        if (user != null)
                            using (var userGroups = user.GetGroups())
                            {
                                foreach (var group in userGroups)
                                {
                                    using (group)
                                    {
                                        if (group.Name.ToLower() == "dashboards")
                                            continue;
                                        if (!grupos.ContainsKey(group.Name.ToLower()))
                                            grupos.Add(group.Name.ToLower(), group.Name.ToLower());
                                    }
                                }
                            }
                    }
                }

                // 1. Verificamos si es del grupo tics (acceso total)
                bool esTics = grupos.ContainsKey("tics");

                var whereConditions = new List<string>();
                var sqlParams = new Dictionary<string, object>();

                if (!esTics)
                {
                    // 2. Condición base: Dashboards vacíos (públicos)
                    whereConditions.Add("MODULOS is null");
                    whereConditions.Add("trim(MODULOS) = ''");

                    // 3. Convertimos el diccionario a lista para recorrerlo
                    var listaGrupos = grupos.Values.ToList();

                    // 4. Por cada grupo, agregamos un LIKE a la consulta SQL
                    for (int i = 0; i < listaGrupos.Count; i++)
                    {
                        // SAP HANA usa '?' para los parámetros posicionales en la consulta
                        whereConditions.Add($" ',' || lower(REPLACE(MODULOS, ' ', '')) || ',' LIKE '%' || ? || '%' ");
                        sqlParams.Add($"@{i}", $",{listaGrupos[i]},");
                    }
                }

                var ms = new List<Dash>();
                var whereClause = esTics ? "" : $"WHERE {string.Join(" OR ", whereConditions)}";

                var sql = $@"
                 select
                   ID,
                   NOMBRE,
                   URL,
                   MODULOS
                 from
                   JB_DASHBOARDS
                 {whereClause}
                 order by
                   ID
                ";

                var bc = new BaseCore();
                var dt = bc.GetDataTableByQuery(sql, sqlParams);

                // Obtenemos los módulos una sola vez fuera del bucle para no saturar al AD
                var modulosDelUsuario = UserBusiness.GetModulosAcceso(userName);

                foreach (DataRow dr in dt.Rows)
                {
                    var dash = new Dash
                    {
                        id = bc.GetInt(dr["ID"]),
                        nombre = dr["NOMBRE"].ToString(),
                        url = dr["URL"].ToString(),
                        modulosStr = dr["MODULOS"].ToString()
                    };

                    modulosDelUsuario.ForEach(mod => {
                        dash.modulos.Add(new ModulosMsg
                        {
                            Name = mod,
                            Checked = dash.modulosStr.Contains(mod)
                        });
                    });
                    ms.Add(dash);
                }
                return new DashBoardsMsg
                {
                    data = ms
                };
            }
            catch (Exception ex)
            {
                return new DashBoardsMsg
                {
                    error = ex.Message
                };
            }

        }

        public static string deleteDasboard(int id)
        {
            try
            {
                var sql = string.Format(@"
                        delete from JB_DASHBOARDS where ID=?
                    ");
                new BaseCore().Execute(sql, new Dictionary<string, object> {
                    {"@0", id }
                });
                return "ok";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public static Dash SaveDashboard(Dash me)
        {
            try
            {
                string sql;
                var esNuevo = me.id == 0;
                if (esNuevo)
                {
                    sql = string.Format(@"
                        insert into JB_DASHBOARDS(NOMBRE, URL, MODULOS)
                        values('{0}', '{1}', '{2}')
                    ", me.nombre, me.url, me.modulosStr);
                }
                else {
                    sql = string.Format(@"
                        update JB_DASHBOARDS
                            set NOMBRE=?,
                            URL=?,
                            MODULOS=?
                        where
                            ID=?
                    ");
                }
                new BaseCore().Execute(sql, new Dictionary<string, object> {
                    {"@0", me.nombre },
                    {"@1", me.url },
                    {"@2", me.modulosStr },
                    {"@3", me.id }
                });
                if (esNuevo) {
                    sql = "select top 1 ID from JB_DASHBOARDS order by ID desc";
                    me.id = new BaseCore().GetIntScalarByQuery(sql, null);
                }
                return me;
            }
            catch (Exception e)
            {
                return new Dash {
                   error=e.Message
                } ;
            }
            
        }
    }
}
