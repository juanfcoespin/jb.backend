using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using jbp.msg;
using TechTools.Core.Hana;
using System.Data;
using jbp.msg.sap;


namespace jbp.business.hana
{
    public class BodegaBusiness
    {
        public static List<SubNivelBodegaMsg> GetSubnivelesAlmacen()
        {
            var ms = new List<SubNivelBodegaMsg>();
            var sql = @"
               select
                ""Id"",
                ""Codigo"", 
                ""Descripcion""
               from ""JbpVw_SubNivelesAlmacen""
               where 
                ""Descripcion"" is not null
                and ""Codigo"" not like '%SYSTEM%'
                and (
                    ""Descripcion"" like 'PERCHA%'
                    OR ""Descripcion"" like 'NIVEL%'
                    OR ""Descripcion"" like 'SECCION%'
                    OR ""Descripcion"" like 'UBICACIÓN%'
                )
              order by
                ""Codigo""
            ";
            var bc = new BaseCore();
            var dt = bc.GetDataTableByQuery(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows){
                    ms.Add(new SubNivelBodegaMsg{
                        id = bc.GetInt(dr["ID"]),
                        codigo = dr["Codigo"].ToString(),
                        descripcion = dr["Descripcion"].ToString()
                    });
                }
            }
            return ms;
        }

        public static PedidosPorProveedorMsg GetPedidosPorProveedor(string codProveedor)
        {
            var ms = new PedidosPorProveedorMsg();
            try
            {
                var sql = String.Format(@"
                    select 
                     t1.""DocNum"" ""NumPedido"",
                     to_char(t1.""Fecha"",'yyyy-mm-dd')  ""FechaPedido"",
                     t0.""LineNum"",
                     t0.""CodArticulo"",
                     t0.""Articulo"",
                     t0.""Cantidad"" ""CantidadPedido"",
                     sum(ifnull(t3.""Cantidad"", 0)) ""CantidadEntregada""
                    from
                     ""JbpVw_PedidosLinea"" t0 inner join
                     ""JbpVw_Pedidos"" t1 on t1.""Id"" = t0.""IdPedido"" left outer join
                     ""JbpVw_EntradaMercanciaLinea"" t3 on t3.""BaseEntry"" = t0.""IdPedido"" and t3.""BaseLine"" = t0.""LineNum"" left outer join
                     ""JbpVw_EntradaMercancia"" t4 on t4.""Id"" = t3.""IdEntradaMercancia""
                    where
                     lower(t1.""Estado"") like '%abierto%'
                     and t1.""CodProveedor"" = '{0}'
                    group by
                     t1.""DocNum"",
                     t1.""Fecha"",
                     t0.""LineNum"",
                     t0.""CodArticulo"",
                     t0.""Articulo"",
                     t0.""Cantidad""
                ",codProveedor);
                var bc = new BaseCore();
                var dt = bc.GetDataTableByQuery(sql);
                var numPedidoAnterior = "";
                var pedido = new PedidoMsg();
                foreach (DataRow dr in dt.Rows) {
                    var numPedido = dr["NumPedido"].ToString();
                    if (numPedidoAnterior != numPedido) { //Un nuevo Pedido
                        pedido = new PedidoMsg
                        {
                            NumPedido = dr["NumPedido"].ToString(),
                            FechaPedido = dr["FechaPedido"].ToString()
                        };
                        ms.Pedidos.Add(pedido);
                    }
                    var linea = new PedidoLineaMsg{
                        LineNum = bc.GetInt(dr["LineNum"]),
                        CodArticulo = dr["CodArticulo"].ToString(),
                        Articulo = dr["Articulo"].ToString(),
                        CantidadPedido = bc.GetDecimal(dr["CantidadPedido"]),
                        CantidadEntregada = bc.GetDecimal(dr["CantidadEntregada"])
                    };
                    linea.CantidadPendiente = linea.CantidadPedido - linea.CantidadEntregada;
                    pedido.Lineas.Add(linea);
                    numPedidoAnterior = pedido.NumPedido;
                }
            }
            catch (Exception e)
            {
                ms.Error = e.Message;
            }
            return ms;
        }
    }
}
