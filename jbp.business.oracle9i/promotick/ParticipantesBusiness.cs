using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using jbp.msg;
using TechTools.Exceptions;
using TechTools.Core.Oracle9i;
using System.Data;

namespace jbp.business.oracle9i.promotick
{
    public class ParticipantesBusiness
    {
        public List<ParticipantesMsg> GetParticipantes() {
            try
            {
                var ms = GetParticipantesPrincipales();
                if (ms != null && ms.Count > 0) {
                    for (int i = 0; i < ms.Count; i++)
                        ms[i].RucsSecundarios = GetRucsSecundariosPorRucPrincipal(ms[i].Ruc);
                }
                return ms;
            }
            catch (Exception e)
            {
                e = ExceptionManager.GetDeepErrorMessage(e, ExceptionManager.eCapa.Business);
                throw e;
            }
        }

        private List<string> GetRucsSecundariosPorRucPrincipal(string ruc)
        {
            try
            {
                var ms = new List<string>();
                var sql = string.Format(@"
                    select 
                     RUC
                    from
                     gms.TBL_CLIENTES_PUNTOSJB 
                    where
                     RUC_PRIN='{0}'
                ",ruc);
                var dt = new BaseCore().GetDataTableByQuery(sql);
                foreach (DataRow dr in dt.Rows)
                    ms.Add(dr["RUC"].ToString());
                return ms;
            }
            catch (Exception e)
            {
                e = ExceptionManager.GetDeepErrorMessage(e, ExceptionManager.eCapa.Business);
                throw e;
            }
        }

        private List<ParticipantesMsg> GetParticipantesPrincipales()
        {
            try
            {
                var ms = new List<ParticipantesMsg>();
                var sql = @"
                    select 
                     RUC,
                     ID_CATALOGO
                    from
                     gms.TBL_CLIENTES_PUNTOSJB 
                    where
                     RUC=RUC_PRIN
                ";
                var bc = new BaseCore();
                var dt = bc.GetDataTableByQuery(sql);
                foreach (DataRow dr in dt.Rows)
                {
                    ms.Add(new ParticipantesMsg
                    {
                        Ruc = dr["RUC"].ToString(),
                        TipoParticipante = GetTipoParticipante(bc.GetInt(dr["ID_CATALOGO"]))
                    }); ;
                }

                return ms;
            }
            catch (Exception e)
            {
                e = ExceptionManager.GetDeepErrorMessage(e, ExceptionManager.eCapa.Business);
                throw e;
            }
        }

        private static eTipoParticipante GetTipoParticipante(int idCatalogo)
        {
            var tipoParticipante = eTipoParticipante.NoDefinido;
            switch (idCatalogo)
            {
                case 1:
                    tipoParticipante = eTipoParticipante.TipoA;
                    break;
                case 2:
                    tipoParticipante = eTipoParticipante.TipoB;
                    break;
            }
            return tipoParticipante;
        }
    }
}
