using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using jbp.msg;
using TechTools.Core.Hana;
using System.Data;
using jbp.msg.sap;


namespace jbp.business.hana
{
    public class MarketingBusiness
    {
        public static DashBoardsMsg GetDasboards()
        {
            try
            {
                var ms = new List<Dash>();
                var sql = string.Format(@"
                 select
                   ID,
                   NOMBRE,
                   URL,
                   MODULOS
                 from
                   JB_DASHBOARDS
                 order by
                   ID
                "
                );
                var bc = new BaseCore();
                var dt = bc.GetDataTableByQuery(sql);
                foreach (DataRow dr in dt.Rows)
                {
                    var dash = new Dash
                    {
                        id = bc.GetInt(dr["ID"]),
                        nombre = dr["NOMBRE"].ToString(),
                        url = dr["URL"].ToString(),
                        modulosStr = dr["MODULOS"].ToString()
                    };
                    var modulos = UserBusiness.GetModulosAcceso();
                    modulos.ForEach(mod => {
                        dash.modulos.Add(new ModulosMsg { 
                            Name=mod,
                            Checked=dash.modulosStr.Contains(mod)
                        });
                    });
                    ms.Add(dash);
                }
                return new DashBoardsMsg
                {
                    data = ms
                }; 
            }
            catch (Exception ex) {
                return new DashBoardsMsg
                { 
                    error=ex.Message
                };
            }
            
        }

        public static string deleteDasboard(int id)
        {
            try
            {
                var sql = string.Format(@"
                        delete from JB_DASHBOARDS where ID={0}
                    ", id);
                new BaseCore().Execute(sql);
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
                            set NOMBRE='{0}',
                            URL='{1}',
                            MODULOS='{2}'
                        where
                            ID={3}
                    ", me.nombre, me.url, me.modulosStr, me.id);
                }
                new BaseCore().Execute(sql);
                if (esNuevo) {
                    sql = "select top 1 ID from JB_DASHBOARDS order by ID desc";
                    me.id = new BaseCore().GetIntScalarByQuery(sql);
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
