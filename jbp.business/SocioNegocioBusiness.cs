using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using jbp.core;
using jbp.msg;
using TechTools.Utils;
using System.IO;

namespace jbp.business
{
    public class SocioNegocioBusiness
    {
        public static string GetCorreoByRuc(string ruc) {
            return new SocioNegocioCore().GetCorreoByRuc(ruc);
        }
        public static string GetCorreoProveedorByRuc(string ruc) {
            return new BaseCore().GetScalarByQuery(SocioNegocioCore.SqlGetCorreoProveedorByRuc(ruc));
        }
        public static string GetRazonSocialByRuc(string ruc)
        {
            return new BaseCore().GetScalarByQuery(SocioNegocioCore.SqlRazonSocialByRuc(ruc));
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
        //para persistir el ruc para poder actualizar la respuesta del WS
        private string lastRucFilePath = Path.GetTempPath()+ "\\lastRucParticipanteSaved.json";
        public string LastRucParticipanteSaved {
            get {
                if (File.Exists(lastRucFilePath)) {
                    return (string) SerializadorJson.Deserializar(lastRucFilePath, typeof(string));
                }
                return null;
            }
            set {
                SerializadorJson.Serializar(value, lastRucFilePath);
            }
        }
        public  SavedMs SaveParticipante(ParticipantesPuntosMsg me) {
            try
            {
                var ms = new SavedMs();
                var sn = new SocioNegocioCore();
                if (ExisteParticipante(me.NroDocumentoAnterior))
                {
                    ms= sn.UpdateParticipante(me);
                    me.estado = (me.Activo ? 1:-1);
                }
                else
                {
                    ms= sn.InsertParticipante(me);
                    me.estado = 1;
                }
                if(me.nroDocumento==me.RucPrincipal)//solo clientes principales se mandan en el WS
                    SaveParticipanteWsAsync(me);
                return ms;
            }
            catch (Exception e)
            {
                e = ExceptionManager.GetDeepErrorMessage(e, ExceptionManager.eCapa.Business);
                return new SavedMs { Saved = false, Error = e.Message };
            }
        }
        private void SaveParticipanteWsAsync(ParticipantesPuntosMsg me)
        {
            var url = string.Format("{0}/{1}", config.Default.ptkWsUrl, "gstparticipantes");
            var rc = new RestCall();
            rc.DataArrived += (resp,err)=> {
                RespWsMsg respWS = new RespWsMsg();
                if (!string.IsNullOrEmpty(err))
                    respWS.mensaje = err;
                else 
                    respWS = (RespWsMsg)resp;
                SaveRespWSParticipante(respWS);
            };
            var auth = new RestCall.AuthenticationMe() {
                User=config.Default.ptkWsUser,
                Pwd = config.Default.ptkWsPwd,
                AuthType=RestCall.eAuthType.Basic
            };
            LastRucParticipanteSaved = me.nroDocumento;
            rc.SendPostOrPutAsync(url, typeof(RespWsMsg), me, typeof(ParticipantesPuntosMsg), RestCall.eRestMethod.POST, auth);
        }
        private void SaveRespWSParticipante(RespWsMsg respWS)
        {
            var sql = string.Format(@"
            update gms.TBL_CLIENTES_PUNTOSJB
            set CODIGO_RESPWS={1},
            MENSAJE_RESPWS='{2}'
            where ruc='{0}'
            ", LastRucParticipanteSaved,respWS.codigo,respWS.mensaje);
            new BaseCore().Execute(sql);
        }
        private static void Rc_DataArrived(object result, string errorMessage)
        {
            throw new NotImplementedException();
        }
        public static bool ExisteParticipante(string nroDocumento) {
            return new SocioNegocioCore().ExisteParticipante(nroDocumento);
        }
    }
}
