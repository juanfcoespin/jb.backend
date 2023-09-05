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

        public static BoolMs SetCantPesadaComponenteOF(CantPesadaComponenteOF me)
        {
            try
            {
                var camposOf = GetCamposOF(me);
                if(camposOf.Estado != "Liberado")
                    return new BoolMs { 
                        Error = string.Format("La Orden de Fabricación {0} del componente a fraccionar no está en estado liberado!!", me.IdOf) 
                    };
                if (me.CantPesada < camposOf.CantidadPlanificada)
                    return new BoolMs
                    {
                        Error = string.Format("La cantidad pesada ({0}{3}) del articulo {1} no puede ser menor a la planificada ({2}{3})!!",
                        me.CantPesada,
                        me.CodArticulo,
                        camposOf.CantidadPlanificada,
                        camposOf.UnidadMedida)
                    };
                var sql = string.Format(@"
                    update WOR1
                    set U_JB_CANT_PESADA={0}
                    where 
                     ""DocEntry""={1}
                     and ""ItemCode""='{2}'
                
                ", me.CantPesada.ToString().Replace(",","."), me.IdOf, me.CodArticulo);
                new BaseCore().Execute(sql);
                return new BoolMs { ms = true };
            }
            catch (Exception e)
            {
                return new BoolMs { Error = e.Message };
            }
        }

        private static CamposOF GetCamposOF(CantPesadaComponenteOF me)
        {
            var ms = new CamposOF();
            var sql = string.Format(@"
                select 
                 t0.""Estado"",
                 t1.""CantidadPlanificada"",
                 t2.""UnidadMedida""
                from 
                 ""JbpVw_OrdenFabricacion"" t0 inner join
                 ""JbpVw_OrdenFabricacionLinea"" t1 on t1.""IdOrdenFabricacion"" = t0.""Id"" inner join
                 ""JbpVw_Articulos"" t2 on t2.""CodArticulo""=t1.""CodInsumo""
                where
                 t0.""Id""={0}
                 and t1.""CodInsumo""='{1}'
            ", me.IdOf, me.CodArticulo);
            var bc = new BaseCore();
            var dt = bc.GetDataTableByQuery(sql);
            foreach (DataRow dr in dt.Rows) {
                ms.Estado = dr["Estado"].ToString();
                ms.CantidadPlanificada = bc.GetDouble(dr["CantidadPlanificada"]);
                ms.UnidadMedida = dr["UnidadMedida"].ToString();
            }
            return ms;
        }

        public static BodegasMS GetBodegasConUbicaciones()
        {
            
            var ms = new BodegasMS();
            try
            {
                var sql = @"
                select distinct(""CodBodega"") ""CodBodega"" from ""JbpVw_Ubicaciones""
                where 
                 ""CodBodega"" not like '%CUAR%'
                 and ""CodBodega"" not like '%RECH%'
                order by 1
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
                select 
                 distinct ""CodBodega""
                from
                 ""JbpVw_Bodegas"" 
                where 
                 ""CodBodega"" not like '%CUAR%'
                 and ""CodBodega"" not like '%RECH%'
                order by 1
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
                 t1.""Ubicacion"" like '%{0}'
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

        public static object GetUbicacionesYDetArticuloPorLote(string lote)
        {
            var ms=new object();
            try
            {
                var sql = string.Format(@"
                    select 
                     top 1
                     t1.""Id"",
                     t1.""CodArticulo"",
                     t1.""Articulo"",
                     t1.""Lote"",
                      case

                         when t1.""Estado"" = 'Acceso Denegado' then 'FOR-BPH-009 Rev.01 / I-POE-BPH-001'-- cuarentena

                        when t1.""Estado"" = 'Liberado' then 'FOR-CCQ-079 Rev.01 / I-POE-CCQ-033'-- liberado

                        when t1.""Estado"" = 'Bloqueado' then 'FOR-ASC-060 Rev.02 / I-POE-ASC-023'-- Rechazado

                     end ""CodPoe"",
                     t1.""Estado"",
                     t2.""UnidadMedida"",
                     t1.""LoteProveedor"",
                     t1.""Fabricante"",
                     to_char(t1.""FechaFabricacion"", 'yyyy-mm-dd') ""FechaFabricacion"",
                     to_char(t1.""FechaVencimiento"", 'yyyy-mm-dd') ""FechaVencimiento"",
                     to_char(t1.""FechaRetesteo"", 'yyyy-mm-dd') ""FechaRetest"",
                     t3.""Proveedor"",
                     t3.""CondicionAlmacenamiento"",
                     t1.""Bultos"",
                     t1.""Observaciones""
                    from
                    ""JbpVw_Lotes"" t1 inner join
                    ""JbpVw_Articulos"" t2 on t2.""CodArticulo"" = t1.""CodArticulo"" left outer join
                    ""JbpVw_EtiqAproME_MP"" t3 on t3.""Lote"" = t1.""Lote"" and t3.""CodArticulo"" = t1.""CodArticulo""
                    where
                     t1.""Lote"" = '{0}'
                ", lote);
                var bc = new BaseCore();
                var dt=bc.GetDataTableByQuery(sql);
                if (dt == null || dt.Rows.Count == 0) {
                    return new {
                        Error = "No se ha encontrado información de este lote en el inventario"
                    };
                    
                }
                foreach (DataRow dr in dt.Rows) {
                    return new {
                        CodArticulo = dr["CodArticulo"].ToString(),
                        Articulo = dr["Articulo"].ToString(),
                        Lote = dr["Lote"].ToString(),
                        CodPoe = dr["CodPoe"].ToString(),
                        Estado = dr["Estado"].ToString(),
                        UnidadMedida = dr["UnidadMedida"].ToString(),
                        LoteProveedor = dr["LoteProveedor"].ToString(),
                        Fabricante = dr["Fabricante"].ToString(),
                        FechaFabricacion = dr["FechaFabricacion"].ToString(),
                        FechaVencimiento = dr["FechaVencimiento"].ToString(),
                        FechaRetest = dr["FechaRetest"].ToString(),
                        Proveedor = dr["Proveedor"].ToString(),
                        CondicionAlmacenamiento = dr["CondicionAlmacenamiento"].ToString(),
                        Bultos = dr["Bultos"].ToString(),
                        Observaciones = dr["Observaciones"].ToString(),
                        UbicacionesCantidad = GetUbicacionesCantidadLote(dr["Id"].ToString())
                    };
                    
                }
                return ms;
            }
            catch (Exception e)
            {
                return new
                {
                    Error = e.Message
                };
            }
        }

        private static List<object> GetUbicacionesCantidadLote(string idLote)
        {
            var ms = new List<object>();
            try
            {
                var sql = string.Format(@"
                    select 
                     t3.""CodBodega"",
                     t3.""Cantidad"" ""CantBodega"",
                     t1.""Ubicacion"",
                     t0.""Cantidad"" ""CantUbicacion""
                    from
                     ""JbpVw_CantidadesPorLote"" t3 inner join
                     ""JbpVw_Lotes"" t2 on t2.""Id"" = t3.""IdLote"" left outer join
                     ""JbpVw_UbicacionPorLote"" t0 on
                       t0.""IdLote"" = t3.""IdLote""
                       and t0.""CodArticulo"" = t3.""CodArticulo""
                       and t0.""CodBodega"" = t3.""CodBodega"" left Outer join
                     ""JbpVw_Ubicaciones"" t1 on t1.""Id"" = t0.""IdUbicacion""
                    where
                     t3.""IdLote"" = {0}
                     and t3.""Cantidad"" != 0
                     and t0.""Cantidad"" != 0
                ", idLote);
                var bc = new BaseCore();
                var dt = bc.GetDataTableByQuery(sql);
                foreach (DataRow dr in dt.Rows)
                {
                    ms.Add(
                        new 
                        {
                            CodBodega = dr["CodBodega"].ToString(),
                            CantBodega = bc.GetDecimal(dr["CantBodega"]),
                            Ubicacion = dr["Ubicacion"].ToString(),
                            CantUbicacion = bc.GetDecimal(dr["CantUbicacion"],4)
                        }
                    );
                }
            }
            catch { };
            return ms;
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
                     t3.""LoteProveedor"",
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
                            Cantidad = bc.GetDecimal(dr["Cantidad"],4),
                            UnidadMedida = dr["UnidadMedida"].ToString(),
                            Fabricante = dr["Fabricante"].ToString(),
                            Lote = dr["Lote"].ToString(),
                            LoteFabricante = dr["LoteProveedor"].ToString(),
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
                     to_char(t1.""Fecha"", 'yyyy-mm-dd')  ""FechaPedido"",
                     t0.""LineNum"",
                     t0.""CodArticulo"",
                     t0.""Articulo"",
                     round(t0.""Cantidad"", 4) ""CantidadPedido"",
                     t5.""UnidadMedida"",
                     sum(ifnull(t3.""Cantidad"", 0)) ""CantidadEntregada""

                    from
                     ""JbpVw_PedidosLinea"" t0 inner join
                     ""JbpVw_Articulos"" t5 on t5.""CodArticulo"" = t0.""CodArticulo"" inner join
                     ""JbpVw_Pedidos"" t1 on t1.""Id"" = t0.""IdPedido"" left outer join
                     ""JbpVw_EntradaMercanciaLinea"" t3 on t3.""BaseEntry"" = t0.""IdPedido"" and t3.""BaseLine"" = t0.""LineNum"" and t3.""CodArticulo"" = t0.""CodArticulo"" left outer join
                     ""JbpVw_EntradaMercancia"" t4 on t4.""Id"" = t3.""IdEntradaMercancia""
                    where
                     t1.""CodProveedor"" = '{0}'
                     and lower(t1.""Estado"") like '%abierto%'
                     and t0.""LineStatus"" = 'O'--lineas abiertas
                     and(t4.""Cancelado"" is null or t4.""Cancelado"" <> 'Y')
                    group by
                     t1.""DocNum"",
                     to_char(t1.""Fecha"", 'yyyy-mm-dd'),
                     t0.""LineNum"",
                     t0.""CodArticulo"",
                     t0.""Articulo"",
                     round(t0.""Cantidad"", 4),
                     t5.""UnidadMedida""
                    order by t1.""DocNum"" desc
                ", codProveedor);
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
                        CantidadPedido = bc.GetDecimal(dr["CantidadPedido"],4),
                        CantidadEntregada = bc.GetDecimal(dr["CantidadEntregada"],4),
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
