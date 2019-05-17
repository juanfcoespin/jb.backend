using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using jbp.msg;
using System.Data;

namespace jbp.core
{
    public class FacturaPromotickCore:BaseCore
    {
        public FacturaPromotickMsg GetById(int idFactura) {
            var comillas = "\"";
            var sql = string.Format(@"
                    SELECT
                        to_char(fechaFactura,'dd/mm/yyyy') fechaFactura,
                        {0}Numero Factura{0},
                        {0}Descripcion{0},
                        {0}Numero Documento{0},
                        {0}Monto{0},
                        {0}Puntos{0}
                    FROM
                        JBPVW_FACTURASPROMOTICkMASNC
                    where
                    id = {1}
                ", comillas, idFactura);
            var dt = GetDataTableByQuery(sql);
            if (dt.Rows.Count > 0) {
                var dr = dt.Rows[0]; //solo hay un registro por id
                return new FacturaPromotickMsg
                {
                    id = idFactura,
                    fechaFactura = dr[0].ToString(),
                    numFactura = dr[1].ToString(),
                    descripcion = dr[2].ToString(),
                    numDocumento = dr[3].ToString(),
                    montoFactura = GetInt(dr[4]),
                    puntos = GetInt(dr[5])
                };
            }
            return null;
        }
        public List<FacturaPromotickMsg> GetFacturasToSendWsByMonth(string currentMonth)
        {
            var ms = new List<FacturaPromotickMsg>();
            var sql = string.Format(@"
            select
	            fr.id
            from
                jbpvw_facturaresumen fr inner join
                gms.TBL_CLIENTES_PUNTOSJB c on fr.rucsocionegocio=c.ruc left outer  join
                gms.TBLFACTENVIADASPROMOTICK fp on fp.idFactura=fr.id
            where
                fp.idFactura is null
                and fr.monto<>0
                and to_char(fr.fechaFactura,'yyyy-mm')='{0}'
                ", currentMonth);
            var dt = GetDataTableByQuery(sql);
            var index = 0;
            foreach (DataRow dr in dt.Rows) {
                try
                {
                    var idFactura = GetInt(dr["id"]);
                    ms.Add(this.GetById(idFactura));
                    index++;
                }
                catch (Exception e)
                {
                    var err = e.Message;
                }
                
            }
            return ms;
        }
        public void UpdateCodigoRespWS(ParametroSalidaPtkMsg resp)
        {
            if (resp != null)
            {
                var sql = string.Format(@"
                update gms.tblFactEnviadasPromotick
                    set CODIGORESPUESTAWS={0},
                    MSGRESPUESTAWS='{1}'
                where nroFactura='{2}'
                ", resp.codigo, resp.mensaje, resp.numFactura);
                Execute(sql);
            }
        }
        public bool EsFacturaPromotic(int idFactura)
        {
            var sql = string.Format(@"
            select
                count(fr.id)
            from 
                jbpvw_facturaResumen fr inner join
                gms.TBL_CLIENTES_PUNTOSJB c on fr.RUCSOCIONEGOCIO=c.RUC
            where 
                fr.ID={0} 
                and c.estado='activo'
            ", idFactura);
            return GetIntScalarByQuery(sql) > 0;
        }
        public void InsertFactEnviada(FacturaPromotickMsg me) {
            var sql = string.Format(@"
                insert into gms.tblFactEnviadasPromotick(
                    idFactura,
                    fechaFactura,
                    nroFactura,
                    descripcion,
                    nroDocumento,
                    monto,
                    puntos
                )values({0},'{1}','{2}','{3}','{4}',{5},{6} )
            ", me.id,me.fechaFactura,me.numFactura,
            me.descripcion,me.numDocumento,me.montoFactura, me.puntos);
            Execute(sql);
         }
    }
}
