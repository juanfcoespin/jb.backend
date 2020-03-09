using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using jbp.msg;
using System.Data;

namespace jbp.core.promotick
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
            foreach (DataRow dr in dt.Rows) {
                try
                {
                    var idFactura = GetInt(dr["id"]);
                    var documento = this.GetById(idFactura);
                    if(documento!=null)
                        ms.Add(documento);
                }
                catch (Exception e)
                {
                    var err = e.Message;
                }
                
            }
            return ms;
        }
        public void UpdateCodigoRespWS(RespPtkWSFacturasMsg resp)
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
        public  void InsertarAceleradorEnviado(AceleradorMsg me)
        {
            var sql = string.Format(@"
            insert into GMS.TBL_ACELERADORES_ENVIADOS(fecha_insercion,nro_documento,puntos)
            values(sysdate,'{0}',{1})", me.NroDocumento,me.puntos);
            Execute(sql);
        }
        public  void UpdateRespAceleradorWS(RespPtkAcelerador me)
        {
            var sql = string.Format(@"
            update GMS.TBL_ACELERADORES_ENVIADOS
            set cod_resp_ws={0}, mensaje_ws='{1}'
            where nro_documento='{2}'
            ", me.codigo, me.mensaje, me.NroDocumento);
            Execute(sql);
        }
        public List<AceleradorMsg> GetListAceleradores(ParametroAceleradoresMsg me)
        {
            var ms = new List<AceleradorMsg>();
            var sql = string.Format(@"
                select
                 nroDocumento,
                 round(sum(monto)) puntos -- el valor facturado de los productos participantes se convierten en puntos
                from
                (
	                select
		                c.ruc_prin nroDocumento,
		                case
			                when fr.codFactura like 'C01-010%' then -1*df.monto --nota de crédito
			                else df.monto
		                end monto
	                from
		                JBPVW_DETALLEFACTURA df inner join
		                JBPVW_PRODUCTO p on df.IDPRODUCTO=p.id inner join
		                JBPVW_ORDENFACTURA ordf on df.idOrden=ordf.id inner join
		                JBPVW_FACTURARESUMEN fr on ordf.idfactura=fr.id inner join
		                gms.TBL_CLIENTES_PUNTOSJB c on fr.RUCSOCIONEGOCIO=c.ruc
	                where
		                c.estado='activo' and
		                df.MONTO<>0 and
		                to_char(fr.fechaFactura,'yyyy') ='{0}' and
		                to_char(fr.fechaFactura,'mm') in ({1}) and
		                p.codRECURSO in({2})
                )
                group by nroDocumento
            ", me.Año,me.Meses, me.CodigosProductos);
            var dt = GetDataTableByQuery(sql);
            if (dt != null && dt.Rows.Count > 0) {
                foreach (DataRow dr in dt.Rows) {
                    ms.Add(new AceleradorMsg {
                        NroDocumento=dr[0].ToString(),
                        puntos=GetInt(dr[1])
                    });
                }
            }
            return ms;
        }
    }
}
