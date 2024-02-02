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
    public class VendedorPtkBusiness : BaseWSPtk
    {
        public void ActualizacionMasivaVendedores(eTipoOperacionVendedor tipoOperacion)
        {
            var vendedores = GetVendedoresToUpdate();
            vendedores.ForEach(vendedor => {
                vendedor.operacion = (int)tipoOperacion;
                RegistrarVendedor(vendedor);
            });
        }

        private void RegistrarVendedor(VendedorPtkMsg me)
        {
            try
            {
                var errorVendedor = "";
                if (!VendedorValido(me, ref errorVendedor))
                    return;

                var url = string.Format("{0}/{1}", conf.Default.ptkWsUrl2, "gstvendedores");
                var rc = new RestCall();
                var resp = (RespWsMsg)rc.SendPostOrPut(url, typeof(RespWsMsg), me, typeof(VendedorPtkMsg), RestCall.eRestMethod.POST, this.credencialesWsPromotick);

                if (resp != null)
                {
                    RegistrarVendedoresEnLog(me, resp);
                    if(resp.codigo==1)
                        RegistrarVendedorComoSincronizado(me.usuarioVendedor);
                }
            }
            catch (Exception e)
            {
                var strJsonParticipante = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(me);
                e = ExceptionManager.GetDeepErrorMessage(e, ExceptionManager.eCapa.Business);
                EnviarPorCorreo("Error en el registro del vendedor", strJsonParticipante + e.Message);
            }
        }

        private void RegistrarVendedorComoSincronizado(string cedulaVendedor)
        {
            var sql = string.Format(@"
                update OSLP
                 set U_SINCRONIZADO_PTK = 1 
                where 
                 ""U_cedula"" = '{0}'
             ", cedulaVendedor);
            new BaseCore().Execute(sql);
        }

        private void RegistrarVendedoresEnLog(VendedorPtkMsg me, RespWsMsg resp)
        {
            var sql = string.Format(@"
                insert into JBP_MODIFICACION_VENDEDORES(
                    VENDEDOR,
                    USUARIO,
                    CLAVE,
                    CORREO,
                    COD_RESP_PTK,
                    RESP_PTK,
                    FECHA_TX
                )
                VALUES(
                    '{0}',
                    '{1}',
                    '{2}',
                    '{3}',
                     {4},
                    '{5}',
                    CURRENT_TIMESTAMP
                )
             ", me.nombreVendedor,me.usuarioVendedor, me.clave, me.correo,
             resp.codigo, resp.mensaje
             );
            new BaseCore().Execute(sql);
        }

        private bool VendedorValido(VendedorPtkMsg me, ref string errorVendedor)
        {
            var error = false;
            var campos = new List<CamposValidar>();
            var campo = string.Empty;
            var valorCampo = string.Empty;


            if (!ValidacionUtils.ValidaCedula_Ecuador(me.usuarioVendedor))
            {
                campos.Add(new CamposValidar
                {
                    Campo = "Cedula",
                    Msg = "La cédula del vendedor está vacía, es incorrecta o no es real"
                });
                error = true;
            }
            if (!ValidacionUtils.EmailValid(me.correo))
            {
                campos.Add(new CamposValidar
                {
                    Campo = "Correo Electrónico",
                    Msg = "El Correo Electrónico del vendedor es incorrecto"
                });
                error = true;
            }
            if (String.IsNullOrEmpty(me.clave))
            {
                campos.Add(new CamposValidar
                {
                    Campo = "Clave",
                    Msg = "La clave de acceso del vendedor es requerida"
                });
                error = true;
            }
            if (String.IsNullOrEmpty(me.nombreVendedor))
            {
                campos.Add(new CamposValidar
                {
                    Campo = "Nombre Vendedor",
                    Msg = "El nombre del vendedor es requerido"
                });
                error = true;
            }
            if (error)
            {
                var titulo = string.Format("Corregir datos del vendedor {0}", me.nombreVendedor);
                var errores = "";
                campos.ForEach(c => {
                    errores += string.Format("<li><b>{0}</b>: {1}</li>", c.Campo, c.Msg);
                });
                errorVendedor = string.Format(@"
                    <div style=""font - family: Arial, Helvetica, sans - serif; "">
                         <b>{0}</b>
                         <p>Favor ingrese a sap en el maestro de empleado del departamento de ventas y corrija lo siguiente:</p>
                         <ul>
                             {1}
                         </ul>
                     </div>
                ", titulo, errores);
                EnviarPorCorreo(titulo, errorVendedor);
            }
            return !error;
        }

        private List<VendedorPtkMsg> GetVendedoresToUpdate()
        {
            var ms = new List<VendedorPtkMsg>();
            var sql = string.Format(@"
                select
                 ""SlpName"" ""nombreVendedor"",
                 ""U_cedula"" ""usuarioVendedor"",
                 ""U_clavePtk"" ""clave"",
                 ""Email"" ""correo""
                from
                 OSLP
                where
                 ""Email"" is not null
                 and(
                  U_SINCRONIZADO_PTK is null
                  or U_SINCRONIZADO_PTK = 0 --NO
                )
            ");
            var dt = new BaseCore().GetDataTableByQuery(sql);
            if (dt.Rows.Count > 0)
            {
                foreach(DataRow dr in dt.Rows)
                {
                    ms.Add(new VendedorPtkMsg {
                        nombreVendedor= dr["nombreVendedor"].ToString(),
                        usuarioVendedor = dr["usuarioVendedor"].ToString(),
                        clave = dr["clave"].ToString(),
                        correo = dr["correo"].ToString()
                    });
                    
                }
            }
            return ms;
        }
    }
}
