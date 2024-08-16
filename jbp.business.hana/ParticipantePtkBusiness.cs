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
                var me = new EstadoCuentaMe { 
                    nroDocumento = ruc,
                    anio = DateTime.Now.Year.ToString()
                };
                var url = string.Format("{0}/{1}", conf.Default.ptkWsUrl, "estadocuenta");
                var rc = new RestCall();
                var resp = rc.SendPostOrPut(url, typeof(String),
                    me, typeof(EstadoCuentaMe), RestCall.eRestMethod.POST, this.credencialesWsPromotick);
                return resp;
            }
            catch (Exception e)
            {
                e = ExceptionManager.GetDeepErrorMessage(e, ExceptionManager.eCapa.Business);
                EnviarPorCorreo(e.Message, "Jbp-Promotick");
                return null;
            }
        }

      

        public void AsignacionMasivaVendedor()
        {
            var participantes = new List<ParticipanteCampoMsg>();
            var bc = new BaseCore();
            var sql = @"
                select
                 ""Ruc"",
                 t1.""Cedula"" ""Vendedor""
                from
                 ""JbpVw_SocioNegocio"" t0 inner join
                 ""JbpVw_Vendedores"" t1 on t1.""CodVendedor"" = t0.""CodVendedor""
                where
                 t0.""Ruc""=""RucPrincipal""
                 and t0.""AplicaPuntos"" = 'SI'
                 and t0.""CodTipoSocioNegocio"" = 'C'
                 and t0.""SincronizadoConBddPromotick""='NO'
            ";
            var dt = bc.GetDataTableByQuery(sql);
            if (dt.Rows.Count > 0)
            {
                foreach(DataRow dr in dt.Rows)
                {
                    var participante = new ParticipanteCampoMsg();
                    participante.nroDocumento = dr["Ruc"].ToString();
                    participante.listaCamposActualizar.Add(new ListaCamposMsg {
                        nombreCampo = "usuarioVendedor",
                        valor = dr["Vendedor"].ToString()
                    });
                    participantes.Add(participante);
                }
                ActualizarParticipantesPorCampoWsPtk(participantes);
            }
        }

        private void ActualizarParticipantesPorCampoWsPtk(List<ParticipanteCampoMsg> participantes)
        {
            var url = string.Format("{0}/{1}", conf.Default.ptkWsUrl, "gstparticipantes/actualizar");
            var rc = new RestCall();
            participantes.ForEach(participante => {
                var resp = (RespWsMsg)rc.SendPostOrPut(
                    url, typeof(RespWsMsg), participante, typeof(ParticipanteCampoMsg),
                    RestCall.eRestMethod.POST, this.credencialesWsPromotick
                );
                RegistrarLogActualizacionParticipanteCampo(participante, resp);
                if (resp != null && resp.codigo==1) //proceso exitoso
                {
                    new SocioNegocioBusiness().RegistrarParticipanteComoSincronizado(participante.nroDocumento);
                }
            });
            
        }

        private void RegistrarLogActualizacionParticipanteCampo(ParticipanteCampoMsg participante, RespWsMsg resp)
        {
            participante.listaCamposActualizar.ForEach(campo =>
            {
                var sql = string.Format(@"
                    insert into JBP_MODIFICACION_PARTICIPANTES_CAMPO(
                            CEDULA_PARTICIPANTE,
                            CAMPO,
                            VALOR_CAMPO,
                            COD_RESP_WS,
                            RESP_WS,
                            FECHA
                        )
                        VALUES(
                         '{0}', '{1}', '{2}',
                         {3}, '{4}', CURRENT_TIMESTAMP
                        )
                     ",
                    participante.nroDocumento,
                    campo.nombreCampo,
                    campo.valor,
                    resp.codigo,
                    resp.mensaje
                );
                new BaseCore().Execute(sql);
            });
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
             MSG_RESPUESTA_WS,
             DESCRIPCION
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
                    respWs = dr["MSG_RESPUESTA_WS"].ToString(),
                    descripcion = dr["DESCRIPCION"].ToString()
                });
            }
            return ms;
        }

        public void RegistrarParticipante(string ruc, string numFactura=null)
        {
            if (string.IsNullOrEmpty(ruc))
                return;
            var me = GetParticipantePuntosByRucPrincipal(ruc);
            RegistrarParticipante(me);
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
        public void RegistroMasivoParticipantes()
        {
            var participantes = GetParticipantesToUpdate();
            participantes.ForEach(p => RegistrarParticipante(p,false)); //se para como parametro false cuando se va a registrar nuevo participante
        }
        public void ActualizacionMasivaParticipantes()
        {
            var participantes = GetParticipantesToUpdate();
            participantes.ForEach(p => RegistrarParticipante(p, true)); //se para como parametro true para actualizar uno o varios campos del participante
        }
        public void InactivarParticipantes()
        {
            var participantes = GetParticipantesToInactivate();
            participantes.ForEach(p => {
                if (!string.IsNullOrEmpty(p.nombres)) {
                    if(RegistrarParticipante(p, false)) //se pone false para escoger la api de registro / inactivacion 
                        QuitarParticipanteADesactivar(p.nroDocumento);
                }
           });
        }

        private void QuitarParticipanteADesactivar(string ruc)
        {
            var sql = string.Format(@"
                delete from JBP_PARTICIPANTES_A_DESACTIVAR
                where RUC='{0}'
            ",ruc);
            new BaseCore().Execute(sql);
        }

        private List<ParticipantesPuntosMsg> GetParticipantesToInactivate()
        {
            var sql = @"
                select 
                 RUC
                from
                 JBP_PARTICIPANTES_A_DESACTIVAR
                -- where RUC='1311218786'
            ";
            var dtRucs = new BaseCore().GetDataTableByQuery(sql);
            var ms = GetParticipantesByRucs(dtRucs, "RUC", -1); //estado -1 para desactivar
            return ms;
        }

        public List<ParticipantesPuntosMsg> GetParticipantesToUpdate()
        {
            var sql = @"
                select 
                 ""Ruc""
                from
                 ""JbpVw_SocioNegocio""
                where
                 ""AplicaPuntos"" = 'SI'
                 and ""Ruc"" = ""RucPrincipal""
                 and ""SincronizadoConBddPromotick"" = 'NO'
                 --and ""Ruc""='0911221224001'
            ";
            var dtRucs = new BaseCore().GetDataTableByQuery(sql);
            var ms = GetParticipantesByRucs(dtRucs, "Ruc",1); //estado 1 para actualizar o insertar
            return ms;
        }
        public List<ParticipantesPuntosMsg> GetParticipantesByRucs(DataTable dtRucs, string fieldRuc, int estado)
        {
            var ms = new List<ParticipantesPuntosMsg>();
            
            foreach (DataRow dr in dtRucs.Rows)
            {
                var rucPrincipal = dr[fieldRuc].ToString();
                
                var participante = GetParticipantePuntosByRucPrincipal(rucPrincipal);
                if (participante != null && !string.IsNullOrEmpty(participante.RucPrincipal)) {
                    participante.estado = estado;
                    ms.Add(participante);
                }
            }
            return ms;
        }
        public bool RegistrarParticipante(ParticipantesPuntosMsg me, bool actualizar=true) {
            try
            {
                // si actualizar es false quiere decir que voy a registrar participantes
                var errorParticipante = "";
                if (!ParticipanteValido(me, ref errorParticipante))
                    return false;

                this.RucParticipante = me.nroDocumento;

                
                var url = string.Format("{0}/{1}", conf.Default.ptkWsUrl, (actualizar)?"gstparticipantes/actualizar": "gstparticipantes");
                
                var rc = new RestCall();
                //promotick cambió el mensaje de entrada
                var resp = new RespWsMsg();
                if (actualizar) {
                    var newMe = traducirMensaje(me);
                    resp = (RespWsMsg)rc.SendPostOrPut(url, typeof(RespWsMsg), newMe, typeof(UpdateParticipanteMsg), RestCall.eRestMethod.POST, this.credencialesWsPromotick);
                    GestionarRespuestaRegistrarParticipante(resp,  newMe, this.RucParticipante);
                }
                else
                {
                    resp = (RespWsMsg)rc.SendPostOrPut(url, typeof(RespWsMsg), me, typeof(ParticipantesPuntosMsg), RestCall.eRestMethod.POST, this.credencialesWsPromotick);
                    GestionarRespuestaRegistrarParticipante(resp, me, this.RucParticipante);
                }
                RegistrarParticipanteEnLog(me, resp);
                if(resp.codigo == 1)
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                var strJsonParticipante = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(me);
                e = ExceptionManager.GetDeepErrorMessage(e, ExceptionManager.eCapa.Business);
                EnviarPorCorreo("Error en el registro del Participante", strJsonParticipante + e.Message);
            }
            return false;
        }

        private UpdateParticipanteMsg traducirMensaje(ParticipantesPuntosMsg me)
        {
            var ms = new UpdateParticipanteMsg();
            ms.nroDocumento=me.nroDocumento;
            
            ms.listaCamposActualizar.Add(new CamposModificarMsg
            {
                nombreCampo = "estado",
                valor = me.estado.ToString()
            });
            if (!string.IsNullOrEmpty(me.nombres))
            {
                ms.listaCamposActualizar.Add(new CamposModificarMsg
                {
                    nombreCampo = "nombres",
                    valor = me.nombres
                });
            }
            if (!string.IsNullOrEmpty(me.apellidos))
            {
                ms.listaCamposActualizar.Add(new CamposModificarMsg
                {
                    nombreCampo = "apellidos",
                    valor = me.apellidos
                });
            }
            if (!string.IsNullOrEmpty(me.email))
            {
                ms.listaCamposActualizar.Add(new CamposModificarMsg
                {
                    nombreCampo = "email",
                    valor = me.email
                });
            }
            if (me.tipoDocumento>0)
            {
                ms.listaCamposActualizar.Add(new CamposModificarMsg
                {
                    nombreCampo = "tipoDocumento",
                    valor = me.tipoDocumento.ToString()
                });
            }
            if (!string.IsNullOrEmpty(me.nroDocumento))
            {
                ms.listaCamposActualizar.Add(new CamposModificarMsg
                {
                    nombreCampo = "nroDocumento",
                    valor = me.nroDocumento
                });
            }
            if (!string.IsNullOrEmpty(me.clave))
            {
                ms.listaCamposActualizar.Add(new CamposModificarMsg
                {
                    nombreCampo = "clave",
                    valor = me.clave
                });
            }
            if (!string.IsNullOrEmpty(me.fechaNacimiento))
            {
                ms.listaCamposActualizar.Add(new CamposModificarMsg
                {
                    nombreCampo = "fechaNacimiento",
                    valor = me.fechaNacimiento
                });
            }
            if (!string.IsNullOrEmpty(me.celular))
            {
                ms.listaCamposActualizar.Add(new CamposModificarMsg
                {
                    nombreCampo = "celular",
                    valor = me.celular
                });
            }
            if (!string.IsNullOrEmpty(me.telefono))
            {
                ms.listaCamposActualizar.Add(new CamposModificarMsg
                {
                    nombreCampo = "telefono",
                    valor = me.telefono
                });
            }
            if (me.tipoGenero>0)
            {
                ms.listaCamposActualizar.Add(new CamposModificarMsg
                {
                    nombreCampo = "tipoGenero",
                    valor = me.tipoGenero.ToString()
                });
            }
            if (me.idCatalogo>0)
            {
                ms.listaCamposActualizar.Add(new CamposModificarMsg
                {
                    nombreCampo = "idCatalogo",
                    valor = me.idCatalogo.ToString()
                });
            }
            if (me.tipoCatalogo>0)
            {
                ms.listaCamposActualizar.Add(new CamposModificarMsg
                {
                    nombreCampo = "tipoCatalogo",
                    valor = me.tipoCatalogo.ToString()

                });
            }
            if (!string.IsNullOrEmpty(me.vendedor))
            {
                ms.listaCamposActualizar.Add(new CamposModificarMsg
                {
                    nombreCampo = "vendedor",
                    valor = me.vendedor
                });
            }
            if (me.metaAnual>0)
            {
                ms.listaCamposActualizar.Add(new CamposModificarMsg
                {
                    nombreCampo = "metaAnual",
                    valor = me.metaAnual.ToString()
                });
            }
            /*falta implementar esta lógica
             * ms.listaCamposActualizar.Add(new CamposModificarMsg
            {
                nombreCampo = "usuarioVendedor",
                valor = me.usuarioVendedor
            });*/
            return ms;
        }

        public void RegistrarParticipanteEnLog(ParticipantesPuntosMsg me, RespWsMsg resp)
        {
            if (resp == null)
                return;
            var sql = string.Format(@"
                insert into JBP_MODIFICACION_PARTICIPANTES(
                    FECHA, ESTADO, NOMBRES, APELLIDOS,
                    EMAIL, TIPO_DOCUMENTO, NRO_DOCUMENTO,
                    CLAVE, FECHA_NACIMIENTO, CELULAR,
                    TELEFONO, TIPO_GENERO, ID_CATALOGO, 
                    TIPO_CATALOGO, VENDEDOR, META_ANUAL,
                    COD_WS, MSG_WS
                )
                VALUES(
                 CURRENT_TIMESTAMP, {0}, '{1}', '{2}',
                 '{3}', {4}, '{5}',
                 '{6}', '{7}', '{8}',
                 '{9}', {10}, {11},
                  {12}, '{13}', {14},
                  {15}, '{16}'
                )
             ", me.estado, me.nombres, me.apellidos,
              me.email, me.tipoDocumento, me.nroDocumento,
              me.clave, me.fechaNacimiento, me.celular,
              me.telefono, me.tipoGenero, me.idCatalogo,
              me.tipoCatalogo, me.vendedor, me.metaAnual,
              resp.codigo, resp.mensaje
             );
            new BaseCore().Execute(sql);
        }
       
        public bool ParticipanteValido(string rucPrincipal, ref string errorParticipante) {
            var participante = GetParticipantePuntosByRucPrincipal(rucPrincipal);
            if (participante == null || string.IsNullOrEmpty(participante.nroDocumento))
            {
                var msg = "No esta registrado en SAP el cliente:" + rucPrincipal;
                errorParticipante = msg;
                return false;
            }
                
            return ParticipanteValido(participante, ref errorParticipante);
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

        private void GestionarRespuestaRegistrarParticipante(RespWsMsg respWS, object me, string ruc)
        {
            if (respWS!=null && respWS.codigo != 1) //proceso exitoso
            {
                var titulo = "Error en el registro del Participante " + this.RucParticipante;
                var strJsonParticipante = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(me);
                var msg = "<b>Respuesta Servidor:</b> " + respWS.mensaje;
                msg += "<br /><br /><b>Participante:</b><br />" + strJsonParticipante;
                EnviarPorCorreo(titulo, msg);
            }
            else
                new SocioNegocioBusiness().RegistrarParticipanteComoSincronizado(ruc);
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
        public static ParticipantesPuntosMsg GetParticipantePuntosByRucPrincipal(string ruc)
        {
            /*
             No importa si el parámetro ruc es principal o secundario
             el algoritmo trae al principal
             
             Solo trae los participantes activos
             */
            var ms = new ParticipantesPuntosMsg();
            var bc = new BaseCore();
            var sql = string.Format(@"
                select
                 top 1 -- por si hay mas de un secundario
                 t0.""AplicaPuntos"",
                 t0.""Nombre"",
                 t0.""NombreComercial"",
                 t0.""NombrePtk"",
                 t0.""ApellidoPtk"",
                 t0.""EmailPtk"",
                 case
                    when t0.""CodTipoIdentificacion"" = 'C' then 1--cedula
                    when t0.""CodTipoIdentificacion"" = 'R' then 2--ruc
                 end ""TipoDocumento"",
                 t0.""Ruc"",
                 SUBSTRING(t0.""Ruc"", 5, 6) ""Clave"",
                 to_char(t0.""FechaCumpleaños"", 'dd/mm/yyyy') ""FechaNacimiento"",
                 t0.""CelularPtk"",
                 t0.""TelefonoConvencionalPtk"",
                 case
                    when t0.""Genero"" = 'M' then 1
                    when t0.""Genero"" = 'F' then 2
                    else 1--da error en la plataforma de promotick cuando no se envia un valor
                end ""TipoGenero"",
                 case
                    when T0.""MetaCompras"" >= 10000 then 1
                    when T0.""MetaCompras"" < 10000 then 2
                    else 0
                 end ""IdCatalogo"",
                 case
                    when t0.""EsElite"" = 'SI' then 1
                    when t0.""EsElite"" = 'NO' then 2
                 end ""TipoCatalogo"",
                 
                 t1.""Cedula"" ""Vendedor"",
                 
                 t1.""Vendedor"" ""VendedorStr"",
                 
                 t1.""Email"" ""correoVendedor"",
                 
                 t1.""CodVendedor"",
                 
                 t0.""MetaCompras"",
                 t0.""RucPrincipal"",
                 t0.""Comentario""
                from
                 ""JbpVw_SocioNegocio"" t0 inner join--principal
                 ""JbpVw_Vendedores"" t1 on t1.""CodVendedor"" = t0.""CodVendedor""
                where
                 t0.""Ruc"" = t0.""RucPrincipal""
                 and t0.""Ruc"" = '{0}'
                 and t0.""Activo"" = 'Y'
                 and t0.""AplicaPuntos"" = 'SI'
                 and t0.""CodTipoSocioNegocio"" = 'C'
            ", ruc);
            
            var dt = bc.GetDataTableByQuery(sql);
            if (dt.Rows.Count > 0)
            {
                ms.Activo = true;
                ms.nombres = dt.Rows[0]["NombrePtk"].ToString();
                if (string.IsNullOrEmpty(ms.nombres))
                    ms.nombres = dt.Rows[0]["Nombre"].ToString();
                    
                if (!string.IsNullOrEmpty(ms.nombres) && ms.nombres.Length > 32)
                    ms.nombres = ms.nombres.Substring(0, 32); // validación promotick para el nombre
                ms.apellidos = dt.Rows[0]["ApellidoPtk"].ToString();
                if (ms.apellidos==null) //el servicio web de ptk no soporta nulls
                    ms.apellidos = "";
                ms.nombreComercial= dt.Rows[0]["NombreComercial"].ToString();
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
                ms.vendedorStr = dt.Rows[0]["VendedorStr"].ToString();
                ms.correoVendedor = dt.Rows[0]["correoVendedor"].ToString();
                ms.RucPrincipal = dt.Rows[0]["RucPrincipal"].ToString();
                ms.metaAnual = bc.GetInt(dt.Rows[0]["MetaCompras"]);
                ms.idVendedor= dt.Rows[0]["CodVendedor"].ToString();
                ms.Comentario = dt.Rows[0]["Comentario"].ToString();
            }
            return ms;
        }
        public static List<DocumentoParticipanteMsg> GetFacturasYNcByRucPrincipal(string ruc)
        {
            var ms = new List<DocumentoParticipanteMsg>();
            var sql = string.Format(@"
                select 
                 ""TipoDocumento"",
                 ""fechaFactura"",
                 ""mesFactura"",
                 ""NumFolio"",
                 ""Ruc"",
                 ""RucPrincipal"",
                 ""montoFactura"",
                 ""puntos""
                from ""JbpVw_FacturasMasNCParticipantes""
                where ""RucPrincipal"" = '{0}'
                    
            ", ruc);
            var dt = new BaseCore().GetDataTableByQuery(sql);
            if (dt.Rows.Count > 0) {
                foreach(DataRow dr in dt.Rows) {
                    ms.Add(new DocumentoParticipanteMsg
                    {
                        tipoDocumento=dr["TipoDocumento"].ToString(),
                        fechaDocumento = dr["fechaFactura"].ToString(),
                        mesDocumento = dr["mesFactura"].ToString(),
                        numDocumento = dr["NumFolio"].ToString(),
                        ruc = dr["Ruc"].ToString(),
                        rucPrincipal = dr["RucPrincipal"].ToString(),
                        monto = dr["montoFactura"].ToString(),
                        puntos = dr["Puntos"].ToString(),

                    });
                }
            }
            return ms;
        }
        public static ParticipantesPuntosMsg GetParticipantePuntosConDocumentosByRucPrincipal(string ruc)
        {
            var ms = GetParticipantePuntosByRucPrincipal(ruc);
            ms.documentos = GetFacturasYNcByRucPrincipal(ruc);
            return ms;
        }
    }
}
