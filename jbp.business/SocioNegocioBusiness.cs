using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using jbp.core;
using jbp.msg;
using utilities;

namespace jbp.business
{
    public class SocioNegocioBusiness
    {
        public static string GetCorreoByRuc(string ruc) {
            return new SocioNegocioCore().GetCorreoByRuc(ruc);
        }
        public static string GetRazonSocialByRuc(string ruc)
        {
            return new SocioNegocioCore().GetRazonSocialByRuc(ruc);
        }
        public static ParticipantesPuntosMsg GetParticipanteByRuc(string ruc) {
            return new SocioNegocioCore().GetParticipanteByRuc(ruc);
        }
        public static List<SocioNegocioItemMsg> GetItemsBytoken(string token) {
            return new SocioNegocioCore().GetItemsBytoken(token);
        }
        public static List<string> GetVededores()
        {
            return new SocioNegocioCore().GetListVendedores();
        }
        public static SavedMs SaveParticipante(ParticipantesPuntosMsg me) {
            try
            {
                var sn = new SocioNegocioCore();
                if (ExisteParticipante(me.NroDocumentoAnterior))
                    return sn.UpdateParticipante(me);
                else
                    return sn.InsertParticipante(me);
            }
            catch (Exception e)
            {
                e = ExceptionManager.GetDeepErrorMessage(e, ExceptionManager.eCapa.Business);
                return new SavedMs { Saved = false, Error = e.Message };
            }
        }
        public static bool ExisteParticipante(string nroDocumento) {
            return new SocioNegocioCore().ExisteParticipante(nroDocumento);
        }
    }
}
