using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jbp.msg;
using jbp.utils;
using ComunDelegates;

namespace jbp.utils
{
    public class DocumentoFuenteUtils
    {
        public static string GetPlainTextCabeceraByType(eTipoDocFuente tipoDocFuente, FacturaServicioMsg facturaServicio) {
            var ms = string.Empty;
            var datosEmpresa = string.Format("{0};{0};{1}",
                conf.Default.nombreEmpresa,
                conf.Default.rucEmpresa);
            var codDocumentoFuente = GetCodigoDocumentoFuenteByType(tipoDocFuente);
            var sucursal = SucursalUtils.GetDatosSucursalByCodFactura(facturaServicio.CodFactura);
            var datosSucursal = string.Format("{0};{1};{2}",sucursal.Establecimiento,sucursal.CodSucursal,sucursal.CodLinea);
            ms = string.Format("{0};{1};{2}", datosEmpresa,codDocumentoFuente,datosSucursal);
            return ms;
        }

        private static string GetCodigoDocumentoFuenteByType(eTipoDocFuente tipoDocFuente)
        {
            
            switch (tipoDocFuente)
            {
                case eTipoDocFuente.factura:
                    return "01";
                case eTipoDocFuente.facturaServicios:
                    return "01";
                case eTipoDocFuente.NotasCredito:
                    return "04";
                default:
                    return null;
            }
            
        }
    }
}
