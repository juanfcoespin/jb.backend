using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using jbp.core.oracle9i;

using jbp.msg;
using TechTools.DelegatesAndEnums;
using TechTools.Core.Oracle9i;
using System.Data;
using TechTools.Logs;
using TechTools.Exceptions;
using TechTools.Utils;
using TechTools.Net;

namespace jbp.business.oracle9i
{
    public class FacturaBusiness
    {
        public static List<FacturaTrandinaMsg> GetFacturasToSendTrandina()
        {
            try
            {
                var ms = new List<FacturaTrandinaMsg>();
                var facturaTrandinaCore = new FacturaTrandinaCore();
                
                ms = facturaTrandinaCore.GetListByQuery(
                    FacturaTrandinaCore.GetQueryFacturasEnviarATrandina());
                if (ms != null)
                {
                    var detalleFacturaTrandinaCore = new DetalleFacturaTrandinaCore();

                    ms.ForEach(f => {
                        var sql = 
                            DetalleFacturaTrandinaCore.SqlDetalleFacturaTrandinaByIdFactura(f.Id.ToString());
                        f.Detalle = detalleFacturaTrandinaCore.GetListByQuery(sql);
                    });
                }
                return ms;
            }
            catch (Exception e)
            {
                e = ExceptionManager.GetDeepErrorMessage(e, ExceptionManager.eCapa.Business);
                throw e;
            }
        }
        public static bool RegistrarFacturaEnvioTerceros(List<RegistroFacturaTercerosMsg> me) {
            try
            {
                var facturaTrandinaCore = new FacturaTrandinaCore();
                me.ForEach(registroFacturaTercero => {
                    var sql = string.Format(
                        FacturaTrandinaCore.GetQueryRegistroFacturasEnviadoATerceros(),
                        registroFacturaTercero.IdFactura,
                        registroFacturaTercero.TipoTercero);
                    facturaTrandinaCore.Execute(sql);
                });
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool DesregistrarFacturaEnvioTerceros(List<FacturaTrandinaMsg> me)
        {
            try
            {
                var facturaTrandinaCore = new FacturaTrandinaCore();
                var idsFactura = string.Empty;
                var firstEntry = true;
                me.ForEach(f => {
                    if (!firstEntry)
                        idsFactura += ", ";
                    else
                        firstEntry = false;
                    idsFactura += f.Id.ToString();

                });
                var sql = string.Format(
                    FacturaTrandinaCore.GetQueryBorrarRegistroFacturasEnviadoATerceros(),
                    idsFactura);
                facturaTrandinaCore.Execute(sql);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static void InsertDocumentoConAutorizacionSri(string fileName) {
            var f = new FacturaStupendoCore();
            var sql = string.Format(FacturaStupendoCore.SqlInsertArchivosProcesadosStupendo(), fileName);
            f.Execute(sql);
        }
        public static bool DocumentoAutorizadoByFileName(string archivo)
        {
            var f = new FacturaStupendoCore();
            var sql = string.Format(FacturaStupendoCore.SqlNumRegArchivosProcesadosStupendoByFileName(),archivo);
            return f.GetIntScalarByQuery(sql) > 0;
        }
        public static string GetCodFacturaById(string id) {
            var sql = FacturaCore.SqlGetCodFacturaById(id);
            return new BaseCore().GetScalarByQuery(sql);
        }
        public static string GetObservacionById(string id)
        {
            
            var sql = FacturaCore.SqlObservacionFacturaById(id);
            return new BaseCore().GetScalarByQuery(sql);
        }

        public static void RegistrarAutorizacionSRI(string numAutorizacion, string idDocumento)
        {
            var sql = string.Format(@"
                UPDATE COINVCSUMRY 
                SET CMNT = '{0}' WHERE OBJECTID = {1}",
                numAutorizacion, idDocumento);
            new BaseCore().Execute(sql);
        }

        public static string GetBonificacionByIdFacturaIdRecurso(string idFactura, string codRecurso)
        {
            try
            {
                var ms = "";
                var sql = FacturaCore.SqlBonificacionByIdFacturaCodRecurso(idFactura,codRecurso);
                var dt = new BaseCore().GetDataTableByQuery(sql);
                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        ms += "PROMO: "+dr["SQ_CANTIDAD"].ToString();
                    }
                }
                return ms;
            }
            catch (Exception e)
            {
                e = ExceptionManager.GetDeepErrorMessage(e, ExceptionManager.eCapa.Business);
                throw e;
            }
        }
        
        private static List<FacturaMsg> TraducirAFacturaMsg(DataTable dt)
        {
            try
            {
                var ms = new List<FacturaMsg>();
                foreach (DataRow dr in dt.Rows)
                {
                    var fechaFactura = new BaseCore().GetDateTime(dr["FECHAFACT"]).ToString("dd/MM/yyyy");
                    var fechaVencimiento = new BaseCore().GetDateTime(dr["FECHAVENC"]).ToString("dd/MM/yyyy");
                    var factura = new FacturaMsg
                    {
                        IdFactura = Convert.ToInt32(dr["IDFACT"]),
                        IdOrden = Convert.ToInt32(dr["IDORDEN"]),
                        CodOrden = dr["CODORDEN"].ToString(),
                        CodFactura = dr["FACTURA"].ToString(),
                        Sitio = dr["SITIO"].ToString(),
                        FechaFactura = fechaFactura,
                        RazonSocial = dr["RAZONSOCIALCLI"].ToString(),
                        Ruc = dr["RUC"].ToString(),
                        MontoBruto = MoneyUtils.GetMoneyFormat(dr["MONTOBRUTO"].ToString()),
                        Descuento = MoneyUtils.GetMoneyFormat(dr["DESCUENTO"].ToString()),
                        Impuestos = MoneyUtils.GetMoneyFormat(dr["IMPUESTOS"].ToString()),
                        Monto = MoneyUtils.GetMoneyFormat(dr["IMPORTETOTAL"].ToString()),
                        Moneda = dr["MONEDA"].ToString(),
                        TerminoPago = dr["TERMPAGO"].ToString(),
                        ConocidoComo = dr["CONOCIDOCOMO"].ToString(),
                        CiudadEmisionFactura = dr["CIUFACT"].ToString(),
                        DireccionCliente = dr["DIRECCION"].ToString(),
                        CiudadCliente = dr["CIUDAD"].ToString(),
                        TelefonoCliente = dr["TELEFONO"].ToString(),
                        ContactoCliente = dr["CONTACTO"].ToString(),
                        FechaVencimiento = fechaVencimiento,
                        Vendedor = dr["VENDEDOR"].ToString(),
                        mailCliente = dr["MAIL"].ToString()
                    };
                    try
                    {//Este campo solo se setea para facturas no para facturas servicios.
                        factura.NumGuia = dr["NUMGUIA"].ToString();
                    }
                    catch {}
                    ms.Add(factura);
                }
                return ms;
            }
            catch (Exception e)
            {
                e = ExceptionManager.GetDeepErrorMessage(e, ExceptionManager.eCapa.Business);
                throw e;
            }
            
        }
        public static List<FacturaMsg> GetFacturasPorAprobar(eTipoDocFuente tipoDocumento)
        {
            try
            {
                var ms = new List<FacturaMsg>();
                var sql = FacturaCore.SqlFacturasPorAprobar(tipoDocumento);// se incluye las guias si aplica
                var dt = new BaseCore().GetDataTableByQuery(sql);
                ms = TraducirAFacturaMsg(dt);
                
                // si no se ha emitido la guía de remisión no se manda a autorizar
                // la factura y se notifica por correo al personal de ventas
                // no aplica para factura de servicios
                for (int i = ms.Count; i > 0; i--) // se hace un for porque da un error con foreach al usar remove
                {
                    var fact = ms[i - 1];
                    if (string.IsNullOrEmpty(fact.NumGuia) && tipoDocumento.Equals(eTipoDocFuente.factura))
                    {
                        ms.RemoveAt(i - 1);
                        var subject = string.Format("Factura {0} sin guia de remisión", fact.CodFactura);
                        var msg = string.Format("La factura <b>{0}</b> del cliente <b>{1}</b> no se envia para aprobar en stupendo debido a que no se ha emitido guía de remisión", fact.CodFactura, fact.Ruc);
                        if (!MailUtils.Send(conf.Default.mailErrorVentas, subject, msg))
                            new LogUtils().AddLog(string.Format("No se pudo enviar el correo: {0}", msg));
                    }
                };
                return ms;
            }
            catch (Exception e)
            {
                throw ExceptionManager.GetDeepErrorMessage(e, ExceptionManager.eCapa.Business);
            }
        }
        public static string GetNumGuiaByIdFact(int idFactura)
        {
            var sql = FacturaCore.SqlGetNumGuiaByIdFactura(idFactura.ToString());
            return new BaseCore().GetScalarByQuery(sql);
        }
    }
}
