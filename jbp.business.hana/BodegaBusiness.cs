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

        public static BodegasMS GetBodegasConUbicaciones()
        {
            
            var ms = new BodegasMS();
            try
            {
                var sql = @"
                select distinct(""CodBodega"") ""CodBodega"" from ""JbpVw_Ubicaciones"" order by 1
            ";
                var dt = new BaseCore().GetDataTableByQuery(sql);
                foreach (DataRow dr in dt.Rows)
                {
                    ms.Bodegas.Add(dr["CodBodega"].ToString());
                }
            }
            catch (Exception e)
            {
                ms.Error = e.Message;
            }
            return ms;
            
        }
        public static BodegasMS GetBodegas()
        {

            var ms = new BodegasMS();
            try
            {
                var sql = @"
                select distinct ""CodBodega"" from ""JbpVw_Bodegas"" order by 1
            ";
                var dt = new BaseCore().GetDataTableByQuery(sql);
                foreach (DataRow dr in dt.Rows)
                {
                    ms.Bodegas.Add(dr["CodBodega"].ToString());
                }
            }
            catch (Exception e)
            {
                ms.Error = e.Message;
            }
            return ms;

        }

        public static ContenidoUbicacionMS GetContenidoUbicacion(string ubicacion)
        {
            var ms = new ContenidoUbicacionMS();
            try
            {
                var sql = string.Format(@"
                select
                 t2.""Lote"",
                 t0.""CodBodega"",
                 t0.""CodArticulo"",
                 t3.""Articulo"",
                 t0.""Cantidad"",
                 t3.""UnidadMedida""
                from ""JbpVw_UbicacionPorLote"" t0 inner join
                 ""JbpVw_Ubicaciones"" t1 on t1.""Id"" = t0.""IdUbicacion"" inner join
                 ""JbpVw_Lotes"" t2 on t0.""IdLote"" = t2.""Id"" inner join
                 ""JbpVw_Articulos"" t3 on t3.""CodArticulo"" = t0.""CodArticulo""
                where
                 t1.""Ubicacion"" like '%{0}%'
                ",ubicacion);
                var bc = new BaseCore();
                var dt = new BaseCore().GetDataTableByQuery(sql);
                
                foreach (DataRow dr in dt.Rows)
                {
                    ms.Items.Add(new ContenidoUbicacionItemMS
                    {
                        Lote= dr["Lote"].ToString(),
                        CodBodega = dr["CodBodega"].ToString(),
                        CodArticulo = dr["CodArticulo"].ToString(),
                        Articulo = dr["Articulo"].ToString(),
                        Cantidad =  bc.GetDecimal(dr["Cantidad"]),
                        UnidadMedida = dr["UnidadMedida"].ToString(),
                    });
                }
            }
            catch (Exception e)
            {
                ms.Error = e.Message;
            }
            return ms;
        }

        public static UbicacionesMS GetUbicaciones()
        {
            var ms = new UbicacionesMS();
            try
            {
                var sql = @"
                select
                     distinct(""Ubicacion"") ""Ubicacion""
                    from
                     ""JbpVw_Ubicaciones""
                    where
                     ""Ubicacion"" not like '%SYSTEM-BIN-LOCATION%'
                ";
                var dt = new BaseCore().GetDataTableByQuery(sql);
                foreach (DataRow dr in dt.Rows)
                {
                    ms.Ubicaciones.Add(dr["Ubicacion"].ToString());
                }
            }
            catch (Exception e)
            {
                ms.Error = e.Message;
            }
            return ms;
        }

        public static UbicacionesPorLoteMS GetUbicacionesYDetArticuloPorLote(string lote)
        {
            var ms=new UbicacionesPorLoteMS();
            try
            {
                var sql = string.Format(@"
                    select 
                     t0.""CodArticulo"",
                     t2.""Articulo"",
                     t2.""Lote"",
                     t1.""Ubicacion"",
                     t0.""Cantidad""
                    from ""JbpVw_UbicacionPorLote"" t0 inner join
                    ""JbpVw_Ubicaciones"" t1 on t1.""Id"" = t0.""IdUbicacion"" inner join
                    ""JbpVw_Lotes"" t2 on t2.""Id"" = t0.""IdLote""
                    where
                     t2.""Lote"" = '{0}'
                ",lote);
                var bc = new BaseCore();
                var dt=bc.GetDataTableByQuery(sql);
                var seRegistroInfoArticulo = false;
                if (dt == null || dt.Rows.Count == 0) {
                    ms.Error = "No se ha encontrado información de este lote en el inventario";
                    return ms;
                }
                foreach (DataRow dr in dt.Rows) {
                    if (!seRegistroInfoArticulo) {
                        ms.CodArticulo = dr["CodArticulo"].ToString();
                        ms.Articulo = dr["Articulo"].ToString();
                        ms.Lote = dr["Lote"].ToString();
                    }
                    ms.UbicacionesCantidad.Add(
                        new UbicacionCantidad { 
                            Ubicacion= dr["Ubicacion"].ToString(),
                            Cantidad =bc.GetDecimal(dr["Cantidad"])
                        }
                    );
                }
                return ms;
            }
            catch (Exception e)
            {
                ms.Error = e.Message;
                return ms;
            }
        }

        internal static int GetIdLoteByNameAndCodArticulo(string lote, string codArticulo)
        {
            var sql = string.Format(@"
                select ""Id"" from ""JbpVw_Lotes"" 
                where ""Lote""='{0}' and ""CodArticulo""='{1}'
            ", lote, codArticulo);
            return new BaseCore().GetIntScalarByQuery(sql);
        }

        internal static int GetIdUbicacionByName(string ubicacion)
        {
            var sql = string.Format(@"
                select ""Id"" from ""JbpVw_Ubicaciones"" 
                where ""Ubicacion""='{0}'
            ",ubicacion);
            return new BaseCore().GetIntScalarByQuery(sql);
        }

        public static EMProveedorMsg GetEMPorProveedor(string codProveedor)
        {
            var ms = new EMProveedorMsg();
            try
            {
                var sql = String.Format(@"
                    select
                     t1.""DocNum"",
                     t2.""CodArticulo"",
                     t4.""Articulo"",
                     t2.""Cantidad"",
                     t4.""UnidadMedida"",
                     t3.""Fabricante"",
                     t3.""Lote"",
                     t3.""Bultos"",
                     to_char(t3.""FechaIngreso"", 'yyyy-mm-dd') ""FechaIngreso"",
                     to_char(t3.""FechaFabricacion"", 'yyyy-mm-dd') ""FechaFabricacion"",
                     to_char(t3.""FechaVencimiento"", 'yyyy-mm-dd') ""FechaVencimiento"",
                     to_char(t3.""FechaRetesteo"", 'yyyy-mm-dd') ""FechaRetesteo""
                    from
                     ""JbpVw_EntradaMercanciaLinea"" t0 inner join
                     ""JbpVw_EntradaMercancia"" t1 on t1.""Id"" = t0.""IdEntradaMercancia"" inner join
                     ""JbpVw_OperacionesLote"" t2 on t2.""IdDocBase"" = t1.""Id""

                         and t2.""BaseType"" = t1.""IdTipoDocumento""

                         and t2.""CodArticulo"" = t0.""CodArticulo"" inner join
                     ""JbpVw_Lotes"" t3 on t2.""Lote"" = t3.""Lote"" inner join
                     ""JbpVw_Articulos"" t4 on t4.""CodArticulo"" = t2.""CodArticulo""

                    where
                     t1.""IdProveedor"" = '{0}'
                     and t1.""Cancelado"" = 'N'
                     and t3.""Id"" in (--solo las entradas de mercancia que tengan lotes con stock

                         select

                          distinct(""IdLote"")

                         from

                          ""JbpVw_UbicacionPorLote""

                         where ""Cantidad"" > 0
                     )
                    order by
                     t1.""DocNum"" desc
                                    ", codProveedor);
                var bc = new BaseCore();
                var dt = bc.GetDataTableByQuery(sql);
                foreach (DataRow dr in dt.Rows) {
                    ms.EntradasMercancia.Add(
                        new EntradaMercanciaQRMsg
                        {
                            DocNum = dr["DocNum"].ToString(),
                            CodArticulo = dr["CodArticulo"].ToString(),
                            Articulo = dr["Articulo"].ToString(),
                            Cantidad = bc.GetDecimal(dr["Cantidad"]),
                            UnidadMedida = dr["UnidadMedida"].ToString(),
                            Fabricante = dr["Fabricante"].ToString(),
                            Lote = dr["Lote"].ToString(),
                            Bultos = bc.GetInt(dr["Bultos"]),
                            FechaIngreso = dr["FechaIngreso"].ToString(),
                            FechaFabricacion = dr["FechaFabricacion"].ToString(),
                            FechaVencimiento = dr["FechaVencimiento"].ToString(),
                            FechaRetest = dr["FechaRetesteo"].ToString(),
                        }
                    ); ;
                }
            }
            catch (Exception e)
            {
                ms.Error = e.Message;
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
                     t5.""UnidadMedida"",
                     sum(ifnull(t3.""Cantidad"", 0)) ""CantidadEntregada""
                    from
                     ""JbpVw_PedidosLinea"" t0 inner join
                     ""JbpVw_Pedidos"" t1 on t1.""Id"" = t0.""IdPedido"" left outer join
                     ""JbpVw_EntradaMercanciaLinea"" t3 on t3.""BaseEntry"" = t0.""IdPedido"" and t3.""BaseLine"" = t0.""LineNum"" left outer join
                     ""JbpVw_EntradaMercancia"" t4 on t4.""Id"" = t3.""IdEntradaMercancia"" inner join
                     ""JbpVw_Articulos"" t5 on t5.""CodArticulo""=t0.""CodArticulo""
                    where
                     t1.""CodProveedor"" = '{0}'
                     and lower(t1.""Estado"") like '%{1}%'
                     and t0.""LineStatus""='{2}' --lineas abiertas
                    group by
                     t1.""DocNum"",
                     t1.""Fecha"",
                     t0.""LineNum"",
                     t0.""CodArticulo"",
                     t0.""Articulo"",
                     t0.""Cantidad"",
                     t5.""UnidadMedida""
                    order by t1.""DocNum""
                ", codProveedor, "abierto", "O");
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
                        CantidadEntregada = bc.GetDecimal(dr["CantidadEntregada"]),
                        UnidadMedida = dr["UnidadMedida"].ToString(),
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
