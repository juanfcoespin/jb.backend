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
    public class SolicitudTransferenciaBusiness
    {
        public static List<ST_OF_LiberadasMsg> GetST_OF_Liberadas()
        {
            /*
             Son las St correspondientes a las OF liberadas que usualmente se requiere para hacer el picking 
             de  producción desde el app de gestión de bodegas 
            */
            var ms = new List<ST_OF_LiberadasMsg>();
            var sql = @"
               select 
                 top 150
                 t0.""Id"",   
                 t0.""DocNum"" ""NumST"",
                 t1.""DocNum"" ""NumOF"",
                 to_char(t1.""FechaInicio"", 'yyyy-mm-dd') ""FechaInicioOf"",
                 t1.""Articulo"",
                 t1.""CantidadPlanificada"",
                 t1.""UnidadMedida"",
                 t0.""BodegaOrigen"",
                 t0.""BodegaDestino""
                from
                 ""JbpVw_SolicitudTraslado"" t0 inner join
                 ""JbpVw_OrdenFabricacion"" t1 on cast(t1.""DocNum"" as nvarchar(50)) = t0.""DocNumOrdenFabricacion""
                where
                 t0.""DocStatus"" = 'O'--Abierto
                  and t1.""Estado"" = 'Liberado'
                  and t1.""FraccionadoPesaje"" != 'SI'  
                order by
                 t1.""Id"" desc
            ";
            var bc = new BaseCore();
            var dt = bc.GetDataTableByQuery(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows){
                    ms.Add(new ST_OF_LiberadasMsg
                    {
                        Id= bc.GetInt(dr["Id"]),
                        NumST = bc.GetInt(dr["NumST"]),
                        NumOF = bc.GetInt(dr["NumOF"]),
                        FechaInicioOf = dr["FechaInicioOf"].ToString(),
                        Articulo = dr["Articulo"].ToString(),
                        CantidadPlanificada = bc.GetDecimal(dr["CantidadPlanificada"]),
                        UnidadMedida = dr["UnidadMedida"].ToString(),
                        BodegaOrigen = dr["BodegaOrigen"].ToString(),
                        BodegaDestino = dr["BodegaDestino"].ToString(),
                        Componentes = GetComponetesById(bc.GetInt(dr["Id"]))
                    });
                }
            }
            return ms;
        }
        public static List<ST_ComponentesMsg> GetComponetesById(int id)
        {
            var ms = new List<ST_ComponentesMsg>();
            var sql = string.Format(@"
               select 
                 t0.""CodArticulo"",
                 t0.""Articulo"",
                 t0.""CantidadAbierta"" ""Cantidad"",
                 t1.""UnidadMedida"",
                 t0.""BodegaDestino"",
                 t0.""LineNum""
                from
                 ""JbpVw_SolicitudTrasladoLinea"" t0 inner join
                 ""JbpVw_Articulos"" t1 on t1.""CodArticulo"" = t0.""CodArticulo""
                where
                 t0.""IdSolicitudTraslado"" = {0}
                 and t0.""LineStatus""='O' --abierto
            ", id);
            var bc = new BaseCore();
            var dt = bc.GetDataTableByQuery(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    ms.Add(new ST_ComponentesMsg
                    {
                        CodArticulo = dr["CodArticulo"].ToString(),
                        Articulo = dr["Articulo"].ToString(),
                        Cantidad = bc.GetDecimal(dr["Cantidad"]),
                        UnidadMedida = dr["UnidadMedida"].ToString(),
                        BodegaDestino = dr["BodegaDestino"].ToString(),
                        LineNum = bc.GetInt(dr["LineNum"]),
                    });
                }
            }
            return ms;
        }
        public static List<ST_ComponentesMsg> GetComponetesConLotesById(int id)
        {
            var ms = new List<ST_ComponentesMsg>();
            var sql = string.Format(@"
               select 
                 t0.""CodArticulo"",
                 t0.""Articulo"",
                 t2.""Lote"",
                 t2.""Cantidad"",
                 t1.""UnidadMedida"",
                 t3.""Id"" ""IdLote"",
                 t0.""BodegaOrigen"",
                 t0.""BodegaDestino"",
                 t0.""LineNum""
                from
                 ""JbpVw_SolicitudTrasladoLinea"" t0 inner join
                 ""JbpVw_Articulos"" t1 on t1.""CodArticulo"" = t0.""CodArticulo"" inner join
                 ""JbpVw_OperacionesLote"" t2 on
                     t2.""CodArticulo"" = t1.""CodArticulo"" and
                     t2.""IdDocBase"" = t0.""IdSolicitudTraslado"" left outer join
                 ""JbpVw_Lotes"" t3 on t3.""Lote""=t2.""Lote"" and t3.""CodArticulo""=t2.""CodArticulo""
                where
                 t0.""IdSolicitudTraslado"" = {0}
                 and t0.""LineStatus""='O' --abierto
                 and t2.""DireccionTexto"" = 'Asignada'
            ", id);
            var bc = new BaseCore();
            var dt = bc.GetDataTableByQuery(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    var componente = new ST_ComponentesMsg
                    {
                        CodArticulo = dr["CodArticulo"].ToString(),
                        Articulo = dr["Articulo"].ToString(),
                        Lote = dr["Lote"].ToString(),
                        Cantidad = bc.GetDecimal(dr["Cantidad"]),
                        UnidadMedida = dr["UnidadMedida"].ToString(),
                        BodegaOrigen = dr["BodegaOrigen"].ToString(),
                        BodegaDestino = dr["BodegaDestino"].ToString(),
                        LineNum = bc.GetInt(dr["LineNum"]),
                    };
                    componente.Ubicaciones = GetUbicacionesPorLote(bc.GetInt(dr["IdLote"]));
                    ms.Add(componente);
                }
            }
            return ms;
        }

        private static List<UbicacionLoteMsg> GetUbicacionesPorLote(int idLote)
        {
            var ms = new List<UbicacionLoteMsg>();
            var sql = string.Format(@"
               select 
                 t5.""Ubicacion"",
                 t4.""Cantidad""
                from
                 ""JbpVw_Lotes"" t3  left outer join
                 ""JbpVw_UbicacionPorLote"" t4 on t4.""IdLote""=t3.""Id"" left outer join
                 ""JbpVw_Ubicaciones"" t5 on t5.""Id""=t4.""IdUbicacion""
                where
                 t3.""Id"" = {0}
            ", idLote);
            var bc = new BaseCore();
            var dt = bc.GetDataTableByQuery(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    ms.Add(new UbicacionLoteMsg{
                        Ubicacion = dr["Ubicacion"].ToString(),
                        Cantidad = bc.GetDecimal(dr["Cantidad"]),
                    });
                }
            }
            return ms;
        }

        public static List<ST_ComponentesDetalleMsg> GetDetalleComponetesById(int id)
        {
            var ms = new List<ST_ComponentesDetalleMsg>();
            var sql = string.Format(@"
               select 
                 t0.""CodArticulo"",
                 t0.""Articulo"",
                 t0.""Cantidad"" ""CantidadTotal"",
                 t1.""Lote"",
                 t1.""Cantidad"" ""CantidadLote"",
                 t1.""Bodega"",
                 t4.""Ubicacion""
                from
                 ""JbpVw_SolicitudTrasladoLinea"" t0 left outer join
                 ""JbpVw_OperacionesLote"" t1 on t1.""IdDocBase"" = t0.""IdSolicitudTraslado""

                     and t1.""CodArticulo"" = t0.""CodArticulo""

                     and t1.""NumLineaDocBase"" = t0.""LineNum"" left outer join
                 ""JbpVw_Lotes"" t2 on t2.""Lote"" = t1.""Lote"" and t2.""CodArticulo"" = t1.""CodArticulo"" left outer join
                 ""JbpVw_UbicacionPorLote"" t3 on t3.""IdLote"" = t2.""Id"" and t3.""CodBodega"" = t1.""Bodega"" left outer join
                 ""JbpVw_Ubicaciones"" t4 on t4.""Id"" = t3.""IdUbicacion""
                where
                 t0.""IdSolicitudTraslado"" = {0}
                 and t1.""DireccionTexto"" = 'Asignada'
            ", id);
            var bc = new BaseCore();
            var dt = bc.GetDataTableByQuery(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    ms.Add(new ST_ComponentesDetalleMsg
                    {
                        CodArticulo = dr["CodArticulo"].ToString(),
                        Articulo = dr["Articulo"].ToString(),
                        Cantidad = bc.GetDecimal(dr["CantidadTotal"]),
                        Lote = dr["Lote"].ToString(),
                        CantidadLote = bc.GetDecimal(dr["CantidadLote"]),
                        Bodega = dr["Bodega"].ToString(),
                        Ubicacion = dr["Ubicacion"].ToString(),
                    });
                }
            }
            return ms;
        }
    }
}
