using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jbp.msg;
using TechTools.Core.Hana;
using TechTools.Exceptions;
using System.Data;

namespace jbp.business.hana
{
    public class SocioNegocioBusiness
    {
        public static List<SocioNegocioItemMsg> GetItemsBytoken(string token)
        {
            var ms = new List<SocioNegocioItemMsg>();
            var campos = new List<string>();
            campos.Add(@"""Ruc""");
            campos.Add(@"""Nombre""");
            campos.Add(@"""NombreComercial""");
          
            var searchCondition = new BaseCore().GetSearchCondition(true, campos, token);
            var sql = String.Format(@"
                select 
                 ""Ruc"",
                 case
                     when ""NombreComercial"" is null or ""NombreComercial"" = '' then ""Nombre""
                     else ""Nombre"" || ' - ' || ""NombreComercial""
                 end ""Nombre""
                from ""JbpVw_SocioNegocio""
                where
                 ""CodTipoSocioNegocio"" = 'C' {0}
            ", searchCondition);
            var dt = new BaseCore().GetDataTableByQuery(sql);
            foreach(DataRow dr in dt.Rows)
            {
                ms.Add(new SocioNegocioItemMsg { 
                    Ruc=dr["Ruc"].ToString(),
                    Nombre = dr["Nombre"].ToString()
                });
            }
            return ms;
        }
        public static void ActualizarBanderaSincronizacionSocioNegocioPtk(string rucCliente, bool sincronizado)
        {
            try
            {
                
                var bc = new BaseCore();
                var sql = string.Format(@"
                    update OCRD
                    set ""U_JBP_SincronizadoConBddPromotick""={0}
                    where ""LicTradNum"" = '{1}'
                ",bc.GetBooleanSAP(sincronizado),rucCliente);
                bc.Execute(sql);
            }
            catch (Exception e)
            {
                e = ExceptionManager.GetDeepErrorMessage(e, ExceptionManager.eCapa.Business);
                throw e;
            }
        }

        /// <summary>
        /// trae un solo correo
        /// </summary>
        /// <param name="email">
        /// Ej: juan.espi@yahoo.com, jespin@jbp.com.ec
        /// Ej: juan.espi@yahoo.com; jespin@jbp.com.ec
        /// </param>
        /// <returns>juan.espi@yahoo.com</returns>
        public static string GetUnSoloCorreo(string emails)
        {
            if (!string.IsNullOrEmpty(emails)) {
                var matriz = emails.Split(new char[] { ',', ';', ' ' });
                if (matriz != null && matriz.Length > 0) {
                    foreach (string email in matriz) {
                        if (!string.IsNullOrEmpty(email))
                            return email;
                    }
                }
                    
            }
            return string.Empty;
        }

        internal void SetRucInRucPrincipalNull()
        {
            var rucs = GetRucsConRucPrincipalNull();
            rucs.ForEach(ruc => {
                var sql = string.Format(@"
                 update OCRD
                  set ""U_JBP_RucPrincipal""=""LicTradNum""
                 where
                  U_IXX_APLICA_PUNTOS = 'SI'
                  and ""LicTradNum"" = '{0}'
                ",ruc);
                new BaseCore().Execute(sql);
            });
        }
        internal List<string> GetRucsConRucPrincipalNull()
        {
            var ms = new List<string>();
            var sql = string.Format(@"
                select
                 ""Ruc""
                from
                 ""JbpVw_SocioNegocio""
                where
                 ""AplicaPuntos"" = 'SI'
                 and ""RucPrincipal"" is null
            ");
            var dt = new BaseCore().GetDataTableByQuery(sql);
            foreach (DataRow dr in dt.Rows) {
                ms.Add(dr["Ruc"].ToString());
            }
            return ms;
        }

        public static ParticipantesPuntosMsg GetParticipantePuntosByRuc(string ruc)
        {
            var ms = new ParticipantesPuntosMsg();
            var bc = new BaseCore();
            var sql = string.Format(@"
                select
                 ""Nombre"",
                 ""Email"",
                 case
                    when ""CodTipoIdentificacion"" = 'C' then 1--cedula
                    when ""CodTipoIdentificacion"" = 'R' then 2--ruc
                 end ""TipoDocumento"",
                 ""Ruc"",
                 SUBSTRING(""Ruc"", 5, 6) ""Clave"",
                 to_char(""FechaCumpleaños"", 'dd/mm/yyyy') ""FechaNacimiento"",
                 ""Celular"",
                 ""Telefonos"",
                 case
                    when ""Genero"" = 'M' then 1
                    when ""Genero"" = 'F' then 2
                 end ""TipoGenero"",
                 case
                    when ""TipoCliente"" = 'A' then 1
                    when ""TipoCliente"" = 'B' then 2
                 end ""IdCatalogo"",
                 case
                    when ""EsElite"" = 'SI' then 1
                    when ""EsElite"" = 'NO' then 2
                 end ""TipoCatalogo"",
                 t1.""Vendedor"",
                 t0.""MetaCompras""
                from
                 ""JbpVw_SocioNegocio"" t0 inner join
                 ""JbpVw_Vendedores"" t1 on t1.""CodVendedor"" = t0.""CodVendedor""
                where
                 ""Ruc"" = '{0}'
                 and ""Ruc"" = ""RucPrincipal""
            ", ruc);
            var dt = bc.GetDataTableByQuery(sql);
            if (dt.Rows.Count > 0) {
                ms.nombres= dt.Rows[0]["Nombre"].ToString();
                ms.email = dt.Rows[0]["Email"].ToString();
                ms.tipoDocumento = bc.GetInt(dt.Rows[0]["TipoDocumento"]);
                ms.nroDocumento = dt.Rows[0]["Ruc"].ToString();
                ms.clave = dt.Rows[0]["Clave"].ToString();
                ms.fechaNacimiento = dt.Rows[0]["FechaNacimiento"].ToString();
                ms.celular = dt.Rows[0]["Celular"].ToString();
                ms.telefono = dt.Rows[0]["Telefonos"].ToString();
                ms.tipoGenero= bc.GetInt( dt.Rows[0]["TipoGenero"]);
                ms.idCatalogo = bc.GetInt(dt.Rows[0]["IdCatalogo"]);
                ms.tipoCatalogo= bc.GetInt(dt.Rows[0]["TipoCatalogo"]);
                ms.vendedor = dt.Rows[0]["Vendedor"].ToString();
                ms.metaAnual= bc.GetInt(dt.Rows[0]["MetaCompras"]);
            }
            return ms;
        }
        internal List<ParticipantesPuntosMsg> GetParticipantesPorSincronizar()
        {
            var ms = new List<ParticipantesPuntosMsg>();
            var sql = @"
                select
                 top 10  
                 ""Ruc"",
                 ""SincronizadoConBddPromotick""
                from
                 ""JbpVw_SocioNegocio""
                where
                 ""AplicaPuntos"" = 'SI'
                 and(""SincronizadoConBddPromotick"" is null or ""SincronizadoConBddPromotick"" = 'NO')
            ";
            var dt = new BaseCore().GetDataTableByQuery(sql);
            if (dt.Rows.Count > 0) {
                foreach (DataRow dr in dt.Rows) {
                    ms.Add(GetParticipantePuntosByRuc(dr["Ruc"].ToString()));
                }
            }
            return ms;
        }
        internal void RegistrarParticipanteComoSincronizado(string ruc)
        {
            var sql = string.Format(@"
                update OCRD
                 set ""U_JBP_SincronizadoConBddPromotick"" = 1 
                where 
                 ""LicTradNum"" = '{0}'
                 and ""LicTradNum"" = ""U_JBP_RucPrincipal""
             ",ruc);
            new BaseCore().Execute(sql);
        }
    }
}
