using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTools.Rest;
using jbp.msg;
using TechTools.Core.Hana;
using TechTools.Exceptions;
using System.Data;
using TechTools.Utils;


namespace jbp.business.hana
{
    public class ParticipantePtkBusiness: BaseWSPtk
    {
        public string RucParticipante;
        public object GetEstadoCuentaByRuc(string ruc) {
            try
            {
                var me = new RucMsg { nroDocumento = ruc };
                var url = string.Format("{0}/{1}", conf.Default.ptkWsUrl, "estadocuenta");
                var rc = new RestCall();
                var resp = rc.SendPostOrPut(url, typeof(String),
                    me, typeof(RucMsg), RestCall.eRestMethod.POST, this.credencialesWsPromotick);
                return resp;
            }
            catch (Exception e)
            {
                e = ExceptionManager.GetDeepErrorMessage(e, ExceptionManager.eCapa.Business);
                EnviarPorCorreo(e.Message, "Jbp-Promotick");
                return null;
            }
        }

        public List<DocumentoEnviadoMsg> GetDocumentosEnviadosByRuc(string ruc)
        {
            var ms = new List<DocumentoEnviadoMsg>();
            var sql = string.Format(@"
            select 
             TIPO_DOCUMENTO,
             FECHA_TX,
             FECHA_DOCUMENTO,
             FECHA_DOCUMENTO_ORIGINAL,
             NRO_DOCUMENTO,
             MONTO,
             PUNTOS,
             COD_RESPUESTA_WS,
             MSG_RESPUESTA_WS
            from JBP_LOG_ENVIO_DOCUMENTOS_PTK
            where RUC='{0}'
            ", ruc);
            var bc = new BaseCore();
            var dt = bc.GetDataTableByQuery(sql);
            foreach (DataRow dr in dt.Rows)
            {
                ms.Add(new DocumentoEnviadoMsg { 
                    tipoDocumento=dr["TIPO_DOCUMENTO"].ToString(),
                    fechaEnvio = bc.GetDateTime(dr["FECHA_TX"]).ToString("yyyy-MM-dd"),
                    fechaDocumento = bc.GetDateTime(dr["FECHA_DOCUMENTO"]).ToString("yyyy-MM-dd"),
                    fechaDocumentoOriginal =  (bc.GetDateTime(dr["FECHA_DOCUMENTO_ORIGINAL"]) == DateTime.MinValue)?"":
                        bc.GetDateTime(dr["FECHA_DOCUMENTO_ORIGINAL"]).ToString("yyyy-MM-dd"),
                    nroDocumento = dr["NRO_DOCUMENTO"].ToString(),
                    monto = bc.GetInt(dr["MONTO"]),
                    puntos = bc.GetInt(dr["PUNTOS"]),
                    codRespWS = bc.GetInt(dr["COD_RESPUESTA_WS"]),
                    respWs = dr["MSG_RESPUESTA_WS"].ToString()
                });
            }
            return ms;
        }

        public void RegistrarParticipante(string ruc, string numFactura=null)
        {
            if (string.IsNullOrEmpty(ruc))
                return;
            var me = GetParticipantePuntosByRucPrincipal(ruc);
            RegistrarParticipante(me, numFactura);
        }
        internal void SetRucInRucPrincipalNull()
        {
            var rucs = new SocioNegocioBusiness().GetRucsConRucPrincipalNull();
            rucs.ForEach(ruc => {
                var sql = string.Format(@"
                 update OCRD
                  set ""U_JBP_RucPrincipal""=""LicTradNum""
                 where
                  U_IXX_APLICA_PUNTOS = 'SI'
                  and ""LicTradNum"" = '{0}'
                ", ruc);
                new BaseCore().Execute(sql);
            });
        }
        public void RegistrarParticipante(ParticipantesPuntosMsg me, string numFactura = null) {
            try
            {
                var errorParticipante = "";
                if (!ParticipanteValido(me, ref errorParticipante))
                    return;
                
                this.RucParticipante = me.nroDocumento;
                me.estado = 1; //Para registrarlo en ptk
                var url = string.Format("{0}/{1}", conf.Default.ptkWsUrl, "gstparticipantes");
                var rc = new RestCall();
                var resp = (RespWsMsg)rc.SendPostOrPut(url, typeof(RespWsMsg), me, typeof(ParticipantesPuntosMsg), RestCall.eRestMethod.POST, this.credencialesWsPromotick);
                resp.mensaje = string.Format("NumFactura: {0} {1}", numFactura, resp.mensaje);
                GestionarRespuestaRegistrarParticipante(resp, me);
                new SocioNegocioBusiness().RegistrarParticipanteComoSincronizado(me.nroDocumento);
            }
            catch (Exception e)
            {
                var strJsonParticipante = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(me);
                e = ExceptionManager.GetDeepErrorMessage(e, ExceptionManager.eCapa.Business);
                EnviarPorCorreo("Error en el registro del Participante", strJsonParticipante + e.Message);
            }
            
        }
        public bool ParticipanteValido(string rucPrincipal, ref string errorParticipante) {
            var participante = GetParticipantePuntosByRucPrincipal(rucPrincipal);
            if (participante == null || string.IsNullOrEmpty(participante.nroDocumento))
            {
                var msg = "No esta registrado en SAP el cliente:" + rucPrincipal;
                EnviarPorCorreo("Error envio documento promotick", msg);
                errorParticipante = msg;
                return false;
            }
                
            return ParticipanteValido(participante, ref errorParticipante);
        }
        private class CamposValidar {
            public string Campo { get; set; }
            public string Msg { get; set; }
        }
        public bool ParticipanteValido(ParticipantesPuntosMsg me, ref string errorParticipante)
        {
            
            var error = false;
            var campos = new List<CamposValidar>();
            var campo = string.Empty;
            var valorCampo = string.Empty;
           
                
            if (string.IsNullOrEmpty(me.nombres)) {
                campos.Add(new CamposValidar { 
                    Campo= "NombresPtk",
                    Msg= "Debe registrar el nombre del participante con una longitud máxima de 32 caracteres"
                });
                error = true;
            }
            if (string.IsNullOrEmpty(me.apellidos))
            {
                campos.Add(new CamposValidar
                {
                    Campo = "ApellidosPtk",
                    Msg = "Debe registrar el apellido del participante, Si es una empresa, poner en este campo 'No aplica'"
                });
                error = true;
            }
            if (!ValidacionUtils.EmailValid(me.email))
            {
                campos.Add(new CamposValidar
                {
                    Campo = "EmailPtk",
                    Msg = string.Format("({0}) incorrecto, si el cliente no lo tiene favor poner como correo info@jbp.com.ec", me.email)
                }); ;
                error = true;
            }
            if (string.IsNullOrEmpty(me.RucPrincipal)) {
                campos.Add(new CamposValidar
                {
                    Campo = "RucPrincipal",
                    Msg = "Hay que registrar el ruc principal, si el participante no tiene sucursales, debe registrar en este campo el valor del ruc"
                });
                error = true;
            }
            if (!string.IsNullOrEmpty(me.celular) && !ValidacionUtils.CelularValido(me.celular)) {
                campos.Add(new CamposValidar
                {
                    Campo = "Celular",
                    Msg = string.Format("({0}) incorreco", me.celular)
                }); ;
                error = true;
            }
            if (!string.IsNullOrEmpty(me.telefono) && !ValidacionUtils.TelefonoConvencionalValido(me.telefono))
            {
                campos.Add(new CamposValidar
                {
                    Campo = "Telefono Convencional",
                    Msg = string.Format("({0}) incorrecto, recuerde anteponer el codigo de provincia", me.telefono)
                }); ;
                error = true;
            }
            if (error) {
                var titulo = string.Format("Corregir datos Participante Puntos {0}",me.nroDocumento);
                var errores = "";
                campos.ForEach(c=> {
                    errores += string.Format("<li>{0}: {1}</li>", c.Campo, c.Msg);
                });
                errorParticipante = string.Format(@"
                    <div style=""font - family: Arial, Helvetica, sans - serif; "">
                         <b>{0}</b>
                         <p>Favor ingrese a sap en la categoría promotick y corrija lo siguiente:</p>
                         <ul>
                             {1}
                         </ul>
                     </div>
                ", titulo,errores);
                EnviarPorCorreo(titulo, errorParticipante);
            }
            return !error;
        }

        private void GestionarRespuestaRegistrarParticipante(RespWsMsg respWS, ParticipantesPuntosMsg me)
        {
            if (respWS.codigo != 1) //proceso exitoso
            {
                var titulo = "Error en el registro del Participante " + this.RucParticipante;
                var strJsonParticipante = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(me);
                var msg = "<b>Respuesta Servidor:</b> " + respWS.mensaje;
                msg+= "<br /><br /><b>Participante:</b><br />" + strJsonParticipante;
                EnviarPorCorreo(titulo, msg);
            }
        }
        public string GetRucClienteFromLogByNumDocumento(string numDocumento)
        {
            var sql = string.Format(@"
                select  
                 RUC
                from
                 JBP_LOG_ENVIO_DOCUMENTOS_PTK
                where 
                 NRO_DOCUMENTO='{0}' 
                 and to_char(now(),'yyyy-mm-dd')=to_char(FECHA_TX,'yyyy-mm-dd')
            ", numDocumento);
            var bc = new BaseCore();
            var ms = bc.GetScalarByQuery(sql);
            return ms;
        }
        public void SincronizarDatosDeParticipantes()
        {
            var participantesPorSincronizar = GetParticipantesPorSincronizar();
            var pb = new ParticipantePtkBusiness();
            foreach (var participante in participantesPorSincronizar) {
                pb.RegistrarParticipante(participante);
            }
        }
        public static ParticipantesPuntosMsg GetParticipantePuntosByRucPrincipal(string rucPrincipal)
        {
            var ms = new ParticipantesPuntosMsg();
            var bc = new BaseCore();
            var sql = string.Format(@"
                select
                 ""AplicaPuntos"",
                 ""Nombre"",
                 ""NombrePtk"",
                 ""ApellidoPtk"",
                 ""EmailPtk"",
                 case
                    when ""CodTipoIdentificacion"" = 'C' then 1--cedula
                    when ""CodTipoIdentificacion"" = 'R' then 2--ruc
                 end ""TipoDocumento"",
                 ""Ruc"",
                 SUBSTRING(""Ruc"", 5, 6) ""Clave"",
                 to_char(""FechaCumpleaños"", 'dd/mm/yyyy') ""FechaNacimiento"",
                 ""CelularPtk"",
                 ""TelefonoConvencionalPtk"",
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
                 t0.""MetaCompras"",
                 t0.""RucPrincipal""
                from
                 ""JbpVw_SocioNegocio"" t0 inner join
                 ""JbpVw_Vendedores"" t1 on t1.""CodVendedor"" = t0.""CodVendedor""
                where
                 t0.""RucPrincipal"" = '{0}'
                 and ""AplicaPuntos"" = 'SI'
                 and ""CodTipoSocioNegocio"" = 'C'
            ", rucPrincipal);
            var dt = bc.GetDataTableByQuery(sql);
            if (dt.Rows.Count > 0)
            {
                ms.Activo = dt.Rows[0]["AplicaPuntos"].ToString() == "SI" ? true : false;
                ms.nombres = dt.Rows[0]["NombrePtk"].ToString();
                if (string.IsNullOrEmpty(ms.nombres))
                {
                    ms.nombres = dt.Rows[0]["Nombre"].ToString();
                    
                }
                if (!string.IsNullOrEmpty(ms.nombres) && ms.nombres.Length > 32)
                    ms.nombres = ms.nombres.Substring(0, 32); // validación promotick para el nombre
                ms.apellidos = dt.Rows[0]["ApellidoPtk"].ToString();
                if (ms.apellidos==null) //el servicio web de ptk no soporta nulls
                    ms.apellidos = "";
                ms.email = dt.Rows[0]["EmailPtk"].ToString();
                ms.tipoDocumento = bc.GetInt(dt.Rows[0]["TipoDocumento"]);
                ms.nroDocumento = dt.Rows[0]["Ruc"].ToString();
                ms.clave = dt.Rows[0]["Clave"].ToString();
                ms.fechaNacimiento = dt.Rows[0]["FechaNacimiento"].ToString();
                ms.celular = dt.Rows[0]["CelularPtk"].ToString();
                ms.telefono = dt.Rows[0]["TelefonoConvencionalPtk"].ToString();
                ms.tipoGenero = bc.GetInt(dt.Rows[0]["TipoGenero"]);
                ms.idCatalogo = bc.GetInt(dt.Rows[0]["IdCatalogo"]);
                ms.tipoCatalogo = bc.GetInt(dt.Rows[0]["TipoCatalogo"]);
                ms.vendedor = dt.Rows[0]["Vendedor"].ToString();
                ms.RucPrincipal = dt.Rows[0]["RucPrincipal"].ToString();
                ms.metaAnual = bc.GetInt(dt.Rows[0]["MetaCompras"]);
            }
            return ms;
        }
        internal List<ParticipantesPuntosMsg> GetParticipantesPorSincronizar()
        {
            var ms = new List<ParticipantesPuntosMsg>();
            var sql = @"
                select
                 top 100  
                 ""Ruc"",
                 ""SincronizadoConBddPromotick""
                from
                 ""JbpVw_SocioNegocio""
                where
                 ""AplicaPuntos"" = 'SI'
                 and(""SincronizadoConBddPromotick"" is null or ""SincronizadoConBddPromotick"" = 'NO')
                 and ""Ruc""=""RucPrincipal""
                 and ""CodTipoSocioNegocio""='C'
            ";
            var dt = new BaseCore().GetDataTableByQuery(sql);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    ms.Add(GetParticipantePuntosByRucPrincipal(dr["Ruc"].ToString()));
                }
            }
            return ms;
        }
    }
}
