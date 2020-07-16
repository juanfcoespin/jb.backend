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

        
        internal void RegistrarParticipanteComoSincronizado(string ruc)
        {
            var sql = string.Format(@"
                update OCRD
                 set ""U_JBP_SincronizadoConBddPromotick"" = 1 
                where 
                 ""LicTradNum"" = '{0}'
             ",ruc);
            new BaseCore().Execute(sql);
        }
    }
}
