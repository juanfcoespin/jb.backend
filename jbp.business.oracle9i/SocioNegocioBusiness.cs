using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using jbp.core.oracle9i;
using jbp.msg;
using TechTools.Exceptions;
using System.IO;
using TechTools.Rest;
using TechTools.Serializador;
using TechTools.Core.Oracle9i;

namespace jbp.business.oracle9i
{
    public class SocioNegocioBusiness
    {
        public static string GetCorreoByRuc(string ruc) {
            return new SocioNegocioCore().GetCorreoByRuc(ruc);
        }
        public static string GetCorreoProveedorByRuc(string ruc) {
            return new BaseCore().GetScalarByQuery(SocioNegocioCore.SqlGetCorreoProveedorByRuc(ruc));
        }

        public static ParticipantesPuntosMsg GetParticipanteByRucFromERP(string ruc)
        {
            var ms = new ParticipantesPuntosMsg();
            var sn = GetByRuc(ruc);
            if (sn != null && !string.IsNullOrEmpty(sn.Nombre)) {
                ms.celular = sn.Celular;
                ms.clave = sn.Ruc.Substring(4,6);
                ms.email = sn.Email;
                ms.nombres = sn.Nombre;
                ms.nroDocumento = sn.Ruc;
                ms.telefono = sn.Telefonos;
                ms.tipoDocumento = sn.TipoIdentificacion.ToLower().Trim() == "ruc" ? 2 : 1;
                ms.tipoGenero = sn.Genero.ToLower().Trim() == "masculino" ? 1 : 2;
                ms.vendedor = sn.Vendedor;
                ms.idCatalogo = 2; //por defecto tipo B
            }
            if (sn != null && !string.IsNullOrEmpty(sn.Error))
                ms.Error = sn.Error;
            return ms;
        }
        public static SocioNegocioMsg GetByRuc(string ruc)
        {
            var ms = new SocioNegocioMsg();
            try
            {
                var sql = string.Format(@"
                select
                 t0.ruc,
                 t0.nombre,
                 t0.CONOCIDOCOMO,
                 t0.TIPOSOCIONEGOCIO,
                 t2.telefonos,
                 t2.fax celular,
                 t2.email,
                 t3.vendedor,
                 t2.ciudad,
                 t2.provincia,
                 t2.direccion,
                 t2.tipoContacto,
                 t4.genero,
                 t4.tipoIdentificacion,
                 t4.estadoCivil
                from jbpvw_socionegocio t0 inner join
                 jbpvw_facturara t1 on t1.idsocionegocio=t0.id inner join
                 jbpvw_contacto t2 on t1.idcontacto=t2.id left outer join
                 jbpvw_vendedor t3 on t3.idSocioNegocio=t0.id left outer join
                 jbpvw_infoAdicionalSN t4 on t4.idSocioNegocio=t0.id
                where t0.RUC='{0}'",ruc);
                var bc = new BaseCore();
                var dtSn = bc.GetDataTableByQuery(sql);
                if (dtSn != null && dtSn.Rows.Count > 0) {
                    ms.Ruc = dtSn.Rows[0]["RUC"].ToString();
                    ms.Nombre = dtSn.Rows[0]["NOMBRE"].ToString();
                    ms.ConocidoComo = dtSn.Rows[0]["CONOCIDOCOMO"].ToString();
                    ms.TipoSocioNegocio = dtSn.Rows[0]["TIPOSOCIONEGOCIO"].ToString();
                    ms.Telefonos = dtSn.Rows[0]["telefonos"].ToString();
                    ms.Celular = dtSn.Rows[0]["celular"].ToString();
                    ms.Email = dtSn.Rows[0]["email"].ToString().Trim();
                    ms.Vendedor = dtSn.Rows[0]["vendedor"].ToString();
                    ms.Ciudad = dtSn.Rows[0]["Ciudad"].ToString();
                    ms.Provincia = dtSn.Rows[0]["Provincia"].ToString();
                    ms.Direccion = dtSn.Rows[0]["Direccion"].ToString();
                    ms.TipoContacto = dtSn.Rows[0]["TipoContacto"].ToString();
                    ms.Genero = dtSn.Rows[0]["Genero"].ToString();
                    ms.TipoIdentificacion = dtSn.Rows[0]["TipoIdentificacion"].ToString();
                    ms.EstadoCivil = dtSn.Rows[0]["EstadoCivil"].ToString();
                }
            }
            catch (Exception e)
            {
                e = ExceptionManager.GetDeepErrorMessage(e, ExceptionManager.eCapa.Business);
                ms.Error = e.Message;
            }
            return ms;
        }

        public static string GetRazonSocialProveedorByRuc(string ruc)
        {
            var sql = string.Format(@"
                SELECT AORSOCIAL AS RAZONSOCIAL 
                FROM AOINFOPROVEEDOR AO, FDADDON AD, FDTRADINGPARTNE CLI 
                WHERE 
                CLI.TP = '{0}'
                AND AD.ADDONDEFN = 'INFO_PROVEEDOR' 
                AND CLI.OBJECTID = AD.PARENTOBJECTID 
                AND AO.FDADDONID = AD.OBJECTID
            ",ruc);
            return new BaseCore().GetScalarByQuery(sql);
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
            var url = string.Format("{0}/{1}", conf.Default.ptkWsUrl, "gstparticipantes");
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
                User=conf.Default.ptkWsUser,
                Pwd = conf.Default.ptkWsPwd,
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
