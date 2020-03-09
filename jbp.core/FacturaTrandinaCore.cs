using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using jbp.msg;
using TechTools.Exceptions;
using System.Data;

namespace jbp.core
{
    public class FacturaTrandinaCore : BaseCoreEntity<FacturaTrandinaMsg>
    {
        public FacturaTrandinaCore() {

            this.TranslateDataToListEvent += (dt=> {
                try
                {
                    var ms = new List<FacturaTrandinaMsg>();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        var item = new FacturaTrandinaMsg
                        {
                            Id = Convert.ToInt32( dt.Rows[i]["ID"]),
                            CodigoCliente = dt.Rows[i]["RUCSOCIONEGOCIO"].ToString(),
                            Documento = dt.Rows[i]["CODFACTURA"].ToString(),
                            FechaFactura = (dt.Rows[i]["FECHAFACTURA"]).ToString(),
                            PuntoEntrega = (dt.Rows[i]["ENVIARA"]).ToString()
                        };
                        ms.Add(item);
                    }
                    return ms;
                }
                catch (Exception e)
                {
                    throw ExceptionManager.GetDeepErrorMessage(e, ExceptionManager.eCapa.Core);
                }
            });
        }
        public static string GetQueryBorrarRegistroFacturasEnviadoATerceros() {
            return @"
                delete from gms.tbl_facturasEnviadasATerceros
                where idFactura in({0})";
        }
        public static string GetQueryRegistroFacturasEnviadoATerceros()
        {
            return @"
                insert into gms.tbl_facturasEnviadasATerceros(idFactura,fechaEnvio, enviadoA)
                values({0},sysdate,'{1}')";
        }
        public static string GetQueryFacturasEnviarATrandina() {
            return @"
                    select
	                    fr.id,
	                    fr.RUCSOCIONEGOCIO,
	                    fr.CODFACTURA,
	                    to_char(fr.FECHAFACTURA, 'yyyy-mm-dd') fechaFactura,
	                    c.PROVINCIA || '-' || c.ciudad || '-' || c.DIRECCION as enviarA
                    from 
	                    jbpvw_FacturaResumen fr inner join
	                    JBPVW_SOCIONEGOCIO sn on fr.RUCSOCIONEGOCIO=sn.ruc inner join
	                    JBPVW_ENVIAR_A ea on sn.idEnviarA=ea.id inner join
	                    JBPVW_CONTACTO c on ea.idContacto=c.id
                    where
                    (
	                    RUCSOCIONEGOCIO like '%-60'
	                    or RUCSOCIONEGOCIO like '%-61'
	                    or RUCSOCIONEGOCIO like '%-62'
	                    or RUCSOCIONEGOCIO like '%-63'
	                    or RUCSOCIONEGOCIO like '%-64'
	                    or RUCSOCIONEGOCIO like '%-65'
	                    or RUCSOCIONEGOCIO like '%-66'
	                    or RUCSOCIONEGOCIO like '%-67'
	                    or RUCSOCIONEGOCIO like '%-68'
	                    or RUCSOCIONEGOCIO like '%-69'
                    )
                    and CODFACTURA like '001-020%' --solo bodega quito
                    and to_char(FECHAFACTURA,'yyyy')>=2019
                    and id not in (
	                    select
		                    idFactura
	                    from
		                    GMS.tbl_FacturasEnviadasATerceros
	                    where
		                    enviadoA='TRANDINA'
                    )
                ";
        }
    }
}
