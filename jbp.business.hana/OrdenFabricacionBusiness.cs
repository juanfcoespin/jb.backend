using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTools.Core.Hana;
using System.Data;
using jbp.msg.sap;

namespace jbp.business.hana
{
    public class OrdenFabricacionBusiness
    {
        public static List<OrdenFabricacionLiberadaPesajeMsg> GetOfLiberadasPesaje()
        {
            var ms = new List<OrdenFabricacionLiberadaPesajeMsg>();
            var sql = @"
                select
                 ""DocNum"",
                 ""CodArticulo"",
                 ""Articulo""
                from ""JbpVw_OrdenFabricacion""
                where
                 ""Estado"" = 'Liberado'
                 and (""CodArticulo"" like '450%' or ""CodArticulo"" like '451%')
                 and ""FraccionadoPesaje"" != 'SI'
                order by ""Id"" desc            
            ";
            var bc = new BaseCore();
            var dt = bc.GetDataTableByQuery(sql);
            foreach (DataRow dr in dt.Rows) {
                ms.Add(new OrdenFabricacionLiberadaPesajeMsg() { 
                    NumOrdenFabricacion=bc.GetInt(dr["DocNum"]),
                    CodigoArticulo= dr["CodArticulo"].ToString(),
                    Descripcion= dr["Articulo"].ToString()
                });
            }
            return ms;
        }

        public static OFMasComponentesMsg GetComponentesAPesarOfByDocNum(int docNum)
        {
            var ms = new OFMasComponentesMsg();
            ms.NumOrdenFabricacion=docNum;
            var sql = string.Format(@"
                select 
                 t1.""DocNum"",
                 t1.""CodArticulo"",
                 t1.""Articulo"",
                 t2.""CodInsumo"",
                 t2.""UnidadMedida"",
                 t2.""Insumo"",
                 t0.""CantidadPlanificada"",
                 t1.""Articulo""
                from 
                 ""JbpVw_OrdenFabricacionLinea"" t0 inner join
                 ""JbpVw_OrdenFabricacion"" t1 on t1.""Id""=t0.""IdOrdenFabricacion"" inner join
                 ""JbpVw_Insumos"" t2 on t2.""CodInsumo""=t0.""CodInsumo""
                where
                 t1.""DocNum""={0}
                 and t2.""TipoInsumo""='Artículo'
                 and lower(t2.""UnidadMedida"")='kg' --solo componentes sujetos a pesarse ver si se incluyen litros

            ", docNum);
            var bc = new BaseCore();
            var dt = bc.GetDataTableByQuery(sql);

            foreach (DataRow dr in dt.Rows)
            {
                if (ms.CodArticulo == null) // para registrar la cabecera del mensaje
                {
                    ms.CodArticulo = dr["CodArticulo"].ToString();
                    ms.Descripcion = dr["Articulo"].ToString();
                    var docNumOf = bc.GetInt(dr["DocNum"]);
                    // bodega origen y destino para los componentes fraccionados
                    var bodegasComponentes = GetBodegasComponentes(docNumOf);
                    ms.BodegaDesde = bodegasComponentes.BodegaDesde;
                    ms.BodegaHasta = bodegasComponentes.BodegaHasta;
                }
                var componente = new ComponentesMsg{
                    CodigoArticulo = dr["CodInsumo"].ToString(),
                    UnidadMedida = dr["UnidadMedida"].ToString(),
                    Descripcion = dr["Insumo"].ToString(),
                    CantidadTotal = bc.GetDecimal(dr["CantidadPlanificada"])
                };
                componente.CantidadesPorLote = GetCantidadesPorLote(docNum, componente.CodigoArticulo);
                ms.Componentes.Add(componente);
            }
            return ms;
        }
        public class BodegaComponenteMsg
        {
            public string BodegaDesde { get; internal set; }
            public string BodegaHasta { get; internal set; }
        }

        private static BodegaComponenteMsg GetBodegasComponentes(int docNumOf)
        {
            var ms = new BodegaComponenteMsg();
            var sql = @"
                select 
                 top 1
                 t0.""BodegaDestino"" ""BodegaDesde"",
                 t2.""Bodega"" ""BodegaHasta""
                from
                  ""JbpVw_OrdenFabricacion"" t1 inner join
                  ""JbpVw_OrdenFabricacionLinea"" t2 on t2.""IdOrdenFabricacion"" = t1.""Id"" left outer join
                  ""JbpVw_SolicitudTraslado"" t0 on t0.""DocNumOrdenFabricacion"" = cast(t1.""DocNum"" as nvarchar(50))
                 where
                  t1.""DocNum"" = '1292'
            ";
            var dt = new BaseCore().GetDataTableByQuery(sql);
            foreach(DataRow dr in dt.Rows)
            {
                ms.BodegaDesde=dr["BodegaDesde"].ToString();
                ms.BodegaHasta = dr["BodegaHasta"].ToString();
            }
            return ms;
        }

        private static List<CantidadLoteOFMsg> GetCantidadesPorLote(int docNumOF, string codigoArticulo)
        {
            var ms=new List<CantidadLoteOFMsg>();
            try
            {
                var sql = string.Format(@"
                    call ""JbSP_LotesTransferidosPorArticuloOF""('{0}', '{1}')
                ", docNumOF, codigoArticulo);
                var bc = new BaseCore();
                var dt=bc.GetDataTableByQuery(sql);
                foreach (DataRow dr in dt.Rows) {
                    ms.Add(new CantidadLoteOFMsg
                    {
                        Lote = dr["Lote"].ToString(),
                        Cantidad = bc.GetDecimal(dr["Cantidad"])
                    }); ;
                }
            }
            catch(Exception e){//en vez del lote se inyecta el error
                ms.Add(new CantidadLoteOFMsg
                {
                    Lote = string.Format("Error: {0}", e.Message)
                });
            }
            return ms;
        }

        internal static int GetIdByDocNum(int docNum)
        {
            var sql = string.Format(@"
                select
                ""Id""
                from
                 ""JbpVw_OrdenFabricacion""
                where
                 ""DocNum"" = {0}
            ", docNum);
            return new BaseCore().GetIntScalarByQuery(sql);
        }

        internal static bool EstaLiberada(int idOF)
        {
            var estado = GetEstado(idOF);
            return estado == "Liberado";
        }

        public static string GetEstado(int idOF)
        {
            var sql = string.Format(@"
                select
                ""Estado""
                from
                 ""JbpVw_OrdenFabricacion""
                where
                 ""Id"" = {0}
            ",idOF);
            return new BaseCore().GetScalarByQuery(sql);
        }
        
    }
}
