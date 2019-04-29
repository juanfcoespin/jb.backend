using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Configuration;
using jbp.msg;
using jbp.utils;
using ComunDelegates;
using System.ComponentModel;
using jbp.proxy.wsViaIntegration;
using presentation.utilities;

namespace jbp.proxy
{
    public class FacturaProxy:BaseProxy
    {
        public  void InsertPullFacturaTrandina() {
            var me = new List<FacturaTrandinaMsg>();
            try
            {
                me = GetFacturasAEnviarATrandina();
                if (me == null || me.Count == 0)
                    return;
                // primero se registra para evitar un reproceso de las mismas facturas 
                // en otra instancia del programa
                if(RegistrarFacturaEnvioTerceros(me, eTipoTercero.TRANDINA))
                {
                    var wsTrandina = new wsViaIntegration.ViaIntegrationClient();
                    var credenciales = getCredencialesWsTrandina();
                    //TODO: Cargar en el mensaje el punto de entrega
                    ShowBackgrounMessage("Consumiendo servicio web de trandina ...");
                    var seInserto = wsTrandina.CargaFacturasJamesB(credenciales.usr, credenciales.pwd, Traducir(me));
                    if (seInserto)
                        ShowBackgrounMessage("Facturas insertadas correctamente en trandina :)");
                    else
                    {
                        var msg = "No se insertaron las facturas en trandina :(";
                        ShowBackgrounMessage(msg);
                        LogUtils.addLog(eTipoLog.warning, msg);
                        DesregistrarFacturaEnvioTerceros(me);
                    }
                }
            }
            catch (Exception e)
            {
                DesregistrarFacturaEnvioTerceros(me);
                e = utilities.ExceptionManager.GetDeepErrorMessage(e,utilities.ExceptionManager.eCapa.Proxy);
                LogUtils.addLog(eTipoLog.error, e.Message);
                ShowErrorMessage(e.Message);
            }
        }
        private bool RegistrarFacturaEnvioTerceros(List<FacturaTrandinaMsg> me, eTipoTercero tipoTercero)
        {
            ShowBackgrounMessage("Registrando Facturas Enviadas a trandina ...");
            if (me == null || me.Count == 0)
                return false;
            var listRegFactura = Traducir(me, tipoTercero);
            var rc = new RestCall();
            var url = string.Format("{0}/registrarFacturasEnvioTerceros", Properties.Settings.Default.urlWsJBPFactura);
            var ms= (bool)rc.GetData_ByPutRestMethod(url, typeof(bool),listRegFactura,
                typeof(List<RegistroFacturaTercerosMsg>));
            ShowBackgrounMessage(string.Format("Registro completado: {0}", ms.ToString()));
            return ms;
        }
        private List<RegistroFacturaTercerosMsg> Traducir(List<FacturaTrandinaMsg> me, eTipoTercero tipoTercero=eTipoTercero.NotDefined)
        {
            if (me == null)
                return null;
            var ms = new List<RegistroFacturaTercerosMsg>();
            me.ForEach(f=> 
                ms.Add(
                    new RegistroFacturaTercerosMsg
                    {
                        IdFactura = f.Id,
                        TipoTercero = tipoTercero.ToString()
                    }
                )
            );
            return ms;
        }
        private bool DesregistrarFacturaEnvioTerceros(List<FacturaTrandinaMsg> me)
        {
            ShowBackgrounMessage("Desregistrando Facturas Enviadas a trandina ...");
            if (me == null || me.Count == 0)
                return false;
            var rc = new RestCall();
            var url = string.Format("{0}/desregistrarFacturasEnvioTerceros", Properties.Settings.Default.urlWsJBPFactura);
            
            var ms= (bool)rc.GetData_ByPutRestMethod(url, typeof(bool), me,
                typeof(List<FacturaTrandinaMsg>));
            if (ms)
                ShowBackgrounMessage(string.Format("Eliminados {0} registros", me.Count.ToString()));
            else
            {
                var msg = "No se pudo eliminar los registros enviados";
                ShowBackgrounMessage(msg);
                LogUtils.addLog(eTipoLog.error, msg);
            }
            return ms;   
        }
        public List<FacturaTrandinaMsg> GetFacturasAEnviarATrandina()
        {
            ShowBackgrounMessage("Trayendo facturas para envio a trandina ...");
            var ms = new List<FacturaTrandinaMsg>();
            var rc = new RestCall();
            var url = string.Format("{0}/getListToSendTrandina", Properties.Settings.Default.urlWsJBPFactura);
            ms = (List<FacturaTrandinaMsg>)rc.GetData_ByGetRestMethod(url, typeof(List<FacturaTrandinaMsg>));
            ShowBackgrounMessage(string.Format("Facturas encontradas: {0}", ms.Count));
            return ms;
        }
        private List<Factura> Traducir(List<FacturaTrandinaMsg> me)
        {
            var ms = new List<Factura>();
            me.ForEach(f => {
                var msDetalle = new List<DetalleFactura>();
                f.Detalle.ForEach(df=> {
                    msDetalle.Add(new DetalleFactura {
                        Cantidad=df.Cantidad,
                        CodigoArticulo=df.CodigoArticulo,
                        Subtotal = df.Subtotal
                    });
                });
                ms.Add(new Factura() {
                    CodigoCliente = f.CodigoCliente,
                    Detalle = msDetalle,
                    Documento = f.Documento,
                    FechaFactura = f.FechaFactura,
                    UsuarioRegistro = f.UsuarioRegistro.ToString(),
                    PuntoEntrega = f.PuntoEntrega
                });
                
            });
            return ms;
        }
        private CredencialesTrandinaMsg getCredencialesWsTrandina()
        {
            var ms = new CredencialesTrandinaMsg();

            #pragma warning disable CS0618 // Type or member is obsolete
            ms.usr = ConfigurationSettings.AppSettings["userWsTrandina"];
            ms.pwd = ConfigurationSettings.AppSettings["pwdWsTrandina"];
            ms.CodRegistro = ConfigurationSettings.AppSettings["codRegistroTrandina"];
            return ms;
        }
    }
}
