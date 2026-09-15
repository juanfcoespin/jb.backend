using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jbp.msg;
using TechTools.Core.Hana;
using System.Data;

namespace jbp.business.hana
{
    public class DirectorioBusiness
    {
        private BaseCore baseCore = new BaseCore();
        public List<ContactoMsg> GetDirectorio()
        {
            var ms = new List<ContactoMsg>();
            var sql = @"
            select 
             ID,
             CONTACTO,
             DEPARTAMENTO,
             PLANTA,
             EXT
            from
             JBP_DIRECTORIO_TELEFONICO
            ";
            var dt = baseCore.GetDataTableByQuery(sql,null);
            foreach(DataRow dr in dt.Rows)
            {
                ms.Add(
                new ContactoMsg
                {
                    ID = dr["ID"].ToString(),
                    CONTACTO = dr["CONTACTO"].ToString(),
                    DEPARTAMENTO = dr["DEPARTAMENTO"].ToString(),
                    PLANTA = dr["PLANTA"].ToString(),
                    Ext = dr["EXT"].ToString()
                }
            );
            }
            return ms;
        }

        public void CrearContacto(ContactoMsg contacto)
        {
            var sql = @"
            insert into JBP_DIRECTORIO_TELEFONICO (
             CONTACTO,
             DEPARTAMENTO,
             PLANTA,
             EXT
            )
            values (
             ?,
             ?,
             ?,
             ?
            )
            ";
            var parameters = new Dictionary<string, object>
            {
                { "@0", contacto.CONTACTO },
                { "@1", contacto.DEPARTAMENTO },
                { "@2", contacto.PLANTA },
                { "@3", contacto.Ext }
            };
            baseCore.Execute(sql, parameters);
        }

        public void ActualizarContacto(int id, ContactoMsg contacto)
        {
            var sql = "select count(*) from JBP_DIRECTORIO_TELEFONICO where ID = ?";
            var parameters = new Dictionary<string, object>
            {
                { "@0", id }
            };
            var count = baseCore.GetIntScalarByQuery(sql, parameters);
            if (count == 0)
                throw new Exception("Contacto no encontrado");

            sql = @"
            update JBP_DIRECTORIO_TELEFONICO
            set 
             CONTACTO = ?,
             DEPARTAMENTO = ?,
             PLANTA = ?,
             EXT = ?
            where 
             ID = ?
            ";
            parameters = new Dictionary<string, object>
            {
                { "@0", contacto.CONTACTO },
                { "@1", contacto.DEPARTAMENTO },
                { "@2", contacto.PLANTA },
                { "@3", contacto.Ext },
                { "@4", id }
            };
            baseCore.Execute(sql, parameters);
        }

        public void EliminarContacto(int id)
        {
            var sql = @"
            delete from JBP_DIRECTORIO_TELEFONICO
            where ID = ?
            ";
            var parameters = new Dictionary<string, object>
            {
                { "@0", id }
            };
            baseCore.Execute(sql, parameters);
        }
    }
}
