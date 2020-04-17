using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using jbp.msg;
using TechTools.Exceptions;
using System.Data;
using TechTools.Core.Oracle9i;

namespace jbp.core.oracle9i
{
    public class SocioNegocioCore:BaseCore
    {
        public static string SqlRazonSocialByRuc(string ruc) {
            var ms = string.Format(@"
                 SELECT
                    AORSOCIAL AS RAZONSOCIAL 
                From
                    AOINFOADICSN AO,
                    FDADDON AD,
                    FDTRADINGPARTNE CLI
                Where 
                    CLI.TP = '{0}'
                    AND AD.ADDONDEFN = 'INFOADICSN'
                    AND CLI.OBJECTID = AD.PARENTOBJECTID
                    AND AO.FDADDONID = AD.OBJECTID
            ", ruc);
            return ms;
        }

        public static string SqlGetCorreoProveedorByRuc(string ruc)
        {
            return string.Format(@"
                SELECT 
                 aoEnvioPag AS CORREO
                FROM
                 AOINFOPROVEEDOR AO, FDADDON AD, FDTRADINGPARTNE CLI
                WHERE CLI.TP = '{0}'
                 AND AD.ADDONDEFN = 'INFO_PROVEEDOR' 
                 AND CLI.OBJECTID = AD.PARENTOBJECTID
                 AND AO.FDADDONID = AD.OBJECTID
            ", ruc);
        }

        public List<SocioNegocioItemMsg> GetItemsBytoken(string token)
        {
            try
            {
                var ms = new List<SocioNegocioItemMsg>();
                var dt = GetDataTableByQuery(SqlSocioNegocioItemByToken(token));
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        ms.Add(
                            new SocioNegocioItemMsg()
                            {
                                Ruc = dr["ruc"].ToString(),
                                Nombre = dr["nombre"].ToString(),
                            }
                        );
                    }
                }
                return ms;
            }
            catch (Exception e)
            {
                e = ExceptionManager.GetDeepErrorMessage(e, ExceptionManager.eCapa.Core);
                throw e;
            }
        }
        public SavedMs InsertParticipante(ParticipantesPuntosMsg me)
        {
            var sql = string.Format(@"
            insert into gms.TBL_CLIENTES_PUNTOSJB(
                nombres, apellidos, email, tipo_documento, 
                ruc, ruc_prin, clave, estado, 
                fecha_nacimiento, celular, telefono,
                tipo_genero, id_catalogo, tipo_catalogo,
                vendedor, cupo_anual, comentario
            )values(
                '{0}','{1}','{2}',{3},
                '{4}','{5}','{6}','activo',
                to_date('{7}','yyyy/mm/dd'),'{8}','{9}',
                {10},{11},{12},
                '{13}',{14},'{15}'
            )",
                me.nombres,me.apellidos, me.email, me.tipoDocumento,
                me.nroDocumento, me.RucPrincipal, me.clave,
                me.FechaNacimiento.ToString("yyyy/MM/dd"), me.celular, me.telefono,
                me.tipoGenero, me.idCatalogo, me.tipoCatalogo,
                me.vendedor,me.metaAnual, me.Comentario
                );
            Execute(sql);
            return new SavedMs { Saved = true };
        }

        

        public SavedMs UpdateParticipante(ParticipantesPuntosMsg me)
        {
            var sql = string.Format(@"
                update gms.TBL_CLIENTES_PUNTOSJB
                    set nombres='{0}',
                    apellidos='{1}',
                    email='{2}',
                    tipo_documento={3},
                    ruc='{4}',
                    ruc_prin='{5}',
                    clave='{6}',
                    estado='{7}',
                    fecha_nacimiento=to_date('{8}','yyyy/mm/dd'),
                    celular='{9}',
                    telefono='{10}',
                    tipo_genero={11},
                    id_catalogo={12},
                    tipo_catalogo={13},
                    vendedor='{14}',
                    cupo_Anual={15},
                    comentario='{16}'
                where
                    ruc='{17}'
            ",
                me.nombres,me.apellidos,me.email,me.tipoDocumento,
                me.nroDocumento, me.RucPrincipal,me.clave,
                me.Activo?"activo":"inactivo",
                me.FechaNacimiento.ToString("yyyy/MM/dd"),
                me.celular,me.telefono,me.tipoGenero, me.idCatalogo,
                me.tipoCatalogo,
                me.vendedor, me.metaAnual, me.Comentario,me.NroDocumentoAnterior
            );
            Execute(sql);
            return new SavedMs { Saved = true };
        }
        private string SqlSocioNegocioItemByToken(string token)
        {
            var campos = new List<string>();
            campos.Add("ruc");
            campos.Add("nombre");
            campos.Add("conocidoComo");
            var searchCondition = GetSearchCondition(true, campos, token);
            var ms= string.Format(@"
            select 
	            ruc,
	            case
		            when nombre=CONOCIDOCOMO then nombre
		            when CONOCIDOCOMO='NO VALIDO' then nombre
                    when CONOCIDOCOMO='' then nombre
                    when CONOCIDOCOMO is null then nombre
                    else
                        nombre || ' ('||CONOCIDOCOMO||')'
	            end nombre
            from JBPVW_SOCIONEGOCIO
            where TIPOSOCIONEGOCIO not in ('BANCOS') {0}", searchCondition);
            return ms;
        }
        public bool ExisteParticipante(string nroDocumento)
        {
            var sql = string.Format(
                @"
                select count(*) from gms.TBL_CLIENTES_PUNTOSJB
                where ruc='{0}'",
                nroDocumento);
            return GetIntScalarByQuery(sql) > 0;
        }
        public string GetCorreoByRuc(string ruc)
        {
            return GetScalarByQuery(SqlCorreoByRuc(ruc));
        }
     
        public static string SqlCorreoByRuc(string ruc)
        {
            var ms = string.Format(@"
                SELECT
                        AOCORREOCL AS CORREO 
                    From
                        AOINFOADICSN AO,
                        FDADDON AD,
                        FDTRADINGPARTNE CLI 
                    Where 
                        CLI.TP = '{0}'
                        And AD.ADDONDEFN = 'INFOADICSN' 
                        And CLI.OBJECTID = AD.PARENTOBJECTID 
                        And AO.FDADDONID = AD.OBJECTID 
            ", ruc);
            return ms;
        }
        private static string SqlParticipantePuntosByRuc(string ruc)
        {
            return string.Format(@"
            select 
                case
                    when estado='activo' then 1
                    else 0
                end activo,
                apellidos,	            
                celular,
                clave,
                cupo_anual,
                case
                    when tipo_catalogo=1 then 1
                    when tipo_catalogo=2 then 0
                end elite,                
                email,
                fecha_nacimiento,
                id_catalogo,
                tipo_genero id_genero,                
                tipo_documento id_tipoDocumento,
                nombres,
	            ruc nroDocumento,
	            ruc_prin,
	            telefono,
	            vendedor,
	            comentario
            from gms.TBL_CLIENTES_PUNTOSJB
            where ruc ='{0}'
            ", ruc);
        }
        public ParticipantesPuntosMsg GetParticipanteByRuc(string ruc)
        {
            try
            {
                var ms = new ParticipantesPuntosMsg();
                var dt = GetDataTableByQuery(SqlParticipantePuntosByRuc(ruc));
                if (dt.Rows.Count > 0)
                {
                    ms =new ParticipantesPuntosMsg(){
                        Activo = GetBoolean(dt.Rows[0]["activo"]),
                        apellidos = dt.Rows[0]["apellidos"].ToString(),
                        celular = dt.Rows[0]["celular"].ToString(),
                        clave = dt.Rows[0]["clave"].ToString(),
                        metaAnual = GetInt(dt.Rows[0]["cupo_anual"]),
                        Elite = GetBoolean(dt.Rows[0]["elite"]),
                        email = dt.Rows[0]["email"].ToString(),
                        FechaNacimiento = GetDateTime(dt.Rows[0]["fecha_nacimiento"]),
                        idCatalogo= GetInt(dt.Rows[0]["id_catalogo"]),
                        tipoGenero = GetInt(dt.Rows[0]["id_genero"]),
                        tipoDocumento= GetInt(dt.Rows[0]["id_tipoDocumento"]),
                        nombres = dt.Rows[0]["nombres"].ToString(),
                        nroDocumento = dt.Rows[0]["nroDocumento"].ToString(),
                        RucPrincipal = dt.Rows[0]["ruc_prin"].ToString(),
                        telefono = dt.Rows[0]["telefono"].ToString(),
                        vendedor = dt.Rows[0]["vendedor"].ToString(),
                        Comentario = dt.Rows[0]["comentario"].ToString(),
                    };
                    ms.NroDocumentoAnterior = ms.nroDocumento;
                }
                return ms;
            }
            catch (Exception e)
            {
                e = ExceptionManager.GetDeepErrorMessage(e, ExceptionManager.eCapa.Core);
                throw e;
            }
        }
        public string SqlGetListVendedores() {
            return @"
                select nombre from JBPVW_SOCIONEGOCIO
                where TIPOSOCIONEGOCIO='VENDEDOR'
                order by nombre
            ";
        }
        public List<string> GetListVendedores() {
            var ms = new List<string>();
            var dt = GetDataTableByQuery(SqlGetListVendedores());
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                    ms.Add(dr["nombre"].ToString());
            }
            return ms;
        }
    }
}
