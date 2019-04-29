using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using jbp.core;
using jbp.msg;
using utilities;
using System.Data;
using jbp.utils;

namespace jbp.business
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
        public static List<FacturaServicioMsg> GetFacturasServicioPorAprobar()
        {
            try
            {
                var ms = new List<FacturaServicioMsg>();
                var sql = FacturaCore.SqlFacturasPorAprobar();
                var dt = new BaseCore().GetDataTableByQuery(sql);
                return TraducirAFacturaServicios(dt);
            }
            catch (Exception e)
            {
                throw ExceptionManager.GetDeepErrorMessage(e, ExceptionManager.eCapa.Business);
            }
        }
        private static List<FacturaServicioMsg> TraducirAFacturaServicios(DataTable dt)
        {
            try
            {
                var ms = new List<FacturaServicioMsg>();
                foreach (DataRow dr in dt.Rows)
                {
                    ms.Add(
                        new FacturaServicioMsg
                        {
                            IdFactura = Convert.ToInt32(dr["IDFACT"]),
                            IdOrden = Convert.ToInt32(dr["IDORDEN"]),
                            CodOrden = dr["ORDEN"].ToString(),
                            CodFactura = dr["FACTURA"].ToString(),
                            Sitio = dr["SITIO"].ToString(),
                            FechaFactura = FechaUtils.getStringDate(Convert.ToDateTime(dr["FECHAFACT"]), "dd/mm/yyyy"),
                            RazonSocial = dr["RAZONSOCIALCLI"].ToString(),
                            Ruc = dr["RUC"].ToString(),
                            MontoBruto = MoneyUtils.GetMoneyFormat(dr["ORDEN"].ToString()),
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
                            FechaVencimiento = FechaUtils.getStringDate(Convert.ToDateTime(dr["FECHAVENC"]),"dd/mm/yyyy"),
                            Vendedor=dr["VENDEDOR"].ToString(),
                            mailCliente = dr["MAIL"].ToString()
                        }
                    );
                }
                return ms;
            }
            catch (Exception e)
            {
                e = ExceptionManager.GetDeepErrorMessage(e, ExceptionManager.eCapa.Business);
                throw e;
            }
            
        }
        public static List<FacturaMsg> GetFacturasPorAprobar()
        {
            var ms = new List<FacturaMsg>();
            var facturasServicios = GetFacturasServicioPorAprobar();
            facturasServicios.ForEach(fs=> {
                ms.Add(
                    new FacturaMsg {
                        NumGuia=GetNumGuiaByIdFact(fs.IdFactura),
                        facturaServicio =fs
                    }
                );
            });
            return ms;
        }
        public static string GetNumGuiaByIdFact(int idFactura)
        {
            var sql = FacturaCore.SqlGetNumGuiaByIdFactura(idFactura.ToString());
            return new BaseCore().GetScalarByQuery(sql);
        }
    }
}
