using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using jbp.msg;
using TechTools.Core.Hana;
using System.Data;
using System.Text.RegularExpressions;


namespace jbp.business.hana
{
    public class ProductBusiness
    {
        public static List<ProductMsg> GetSellProducts()
        {
            //solo se ocupa en el app de veterinarios
            var ms = new List<ProductMsg>();
            var sql = @"
               select
                 t0.""CodArticulo"",
                 t0.""Articulo"" 
                from ""JbpVw_Articulos"" t0 inner join
                 ""JbpVw_GrupoArticulos"" t1 on t1.""CodGrupo""=t0.""CodGrupo""
                where 
                 t0.""Vendible"" = 'SI'
                 and ""VerEnMovil"" = 'SI'
                 and upper(t1.""Grupo"") like '%VET%'
            ";
            var bc = new BaseCore();
            var dt = bc.GetDataTableByQuery(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    var codArticulo = dr["CodArticulo"].ToString();
                    var prices = GetPriceListByCodArticulo(codArticulo);
                    if (prices.Count > 0)
                    {
                        var product = new ProductMsg
                        {
                            id = codArticulo,
                            name = dr["Articulo"].ToString(),
                            lotes = GetLotes(codArticulo),
                            prices = prices
                        };
                        product.stock = GetStockByLotes(product.lotes);
                        ms.Add(product);
                    }
                }
            }
            return ms;
        }

        public static object GetStockPt(string codArticulo)
        {
            try
            {
                var lotes = new List<object>();
                var sql = string.Format(@"
                    select 
                     t1.""Lote"",
                     to_char(t1.""FechaFabricacion"", 'yyyy-mm') ""FecFab"",
                     to_char(t1.""FechaVence"", 'yyyy-mm') ""FecVen"",
                     round(""Cantidad"", 0) ""stock"",
                     current_timestamp ""fechaConsulta""
                    from
                     ""JbpVw_UbicacionPorLote"" t0 inner join
                     ""JbpVw_Lotes"" t1 on t1.""Id"" = t0.""IdLote""
                    where
                     t0.""CodArticulo"" = '{0}'
                     and t0.""CodBodega"" = 'PT1'

                ", codArticulo);
                var bc = new BaseCore();
                var dt = bc.GetDataTableByQuery(sql);
                var fechaConsulta = string.Empty;
                foreach (DataRow dr in dt.Rows)
                {
                    if (string.IsNullOrEmpty(fechaConsulta))
                        fechaConsulta = dr["fechaConsulta"].ToString();
                    lotes.Add(new{
                        lote = dr["Lote"].ToString(),
                        fecFab = dr["FecFab"].ToString(),
                        fecVen = dr["FecVen"].ToString(),
                        stock = bc.GetInt(dr["stock"]),
                        
                    });
                }
                return new { 
                    error = string.Empty,
                    fechaConsulta = fechaConsulta,
                    lotes = lotes
                };
            }
            catch (Exception e)
            {
                return new { error=e.Message };
            }
            
        }

        private static decimal GetStockByLotes(List<LoteMsg> lotes)
        {
            var ms = 0;
            lotes.ForEach(lote => ms += lote.cantidad);
            return ms;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="patronCodigo">
        ///  Ej: "'8%852'"
        ///      "'105%', '106%', '110%', '111%'"
        /// </param>
        /// <returns></returns>
        public static List<ArticuloMsg> GetArticulosByPatronCodigo(string patronesCodigo)
        {
            var ms = new List<ArticuloMsg>();
            if (!string.IsNullOrEmpty(patronesCodigo))
            {
                var listPatrones = patronesCodigo.Split(new char[] { ','}).ToList();
                var sql = string.Format(@"
                select
                 ""CodArticulo"",
                 ""Articulo"",
                 ""Estado"",
                 ""UnidadMedida""
                from
                 ""JbpVw_Articulos""
                where");
                var i = 0;
                listPatrones.ForEach(patronCodigo => {
                    if (i > 0)
                        sql += " or ";
                    sql += string.Format(@" ""CodArticulo"" like {0}", patronCodigo);
                    i++;
                });
                var dt = new BaseCore().GetDataTableByQuery(sql);
                foreach (DataRow dr in dt.Rows) {
                    ms.Add(new ArticuloMsg { 
                        Codigo = dr["CodArticulo"].ToString(),
                        UnidadMedida = dr["UnidadMedida"].ToString(),
                        Descripcion = dr["Articulo"].ToString(),
                        Activo = dr["Estado"].ToString()=="Activo"?true:false
                    });                
                }
            }
            return ms;
        }

        private static List<LoteMsg> GetLotes(string codArticulo)
        {
            var ms = new List<LoteMsg>();
            var sql = string.Format(@"
                select
                 distinct
                 t1.""Lote"",
                 t0.""CodBodega"",
                 t0.""Cantidad"",
                 to_char(t1.""FechaFabricacion"", 'yyyy-mm-dd') ""FechaFabricacion"",
                 to_char(t1.""FechaVencimiento"", 'yyyy-mm-dd') ""FechaVencimiento""
                from
                ""JbpVw_CantidadesPorLote"" t0 inner join
                ""JbpVw_Lotes"" t1 on t1.""Id"" = t0.""IdLote""
                where
                 t0.""Cantidad"" > 0
                 and t1.""Estado"" = 'Liberado'
                 and t0.""CodArticulo"" = '{0}'
                 and t0.""CodBodega"" in ('PT2', 'PT4', 'PICK2')
                order by t1.""FechaVencimiento""
            ", codArticulo);
            var bc = new BaseCore();
            var dt = bc.GetDataTableByQuery(sql);
            foreach(DataRow dr in dt.Rows)
            {
                ms.Add(new LoteMsg()
                {
                    lote = dr["Lote"].ToString(),
                    codBodega = dr["CodBodega"].ToString(),
                    cantidad = bc.GetInt(dr["Cantidad"]),
                    fechaFabricacion = dr["FechaFabricacion"].ToString(),
                    fechaVencimiento = dr["FechaVencimiento"].ToString(),
                });
            }
            return ms;
        }

        public static List<PriceListMsg> GetPriceListByCodArticulo(string codArticulo, string listaPrecio=null)
        {
            var ms = new List<PriceListMsg>();
            var sql = string.Format(@"
               select 
                ""Id"",
                ""ListaPrecio"", 
                ""Precio""
               from ""JbpVw_ListaPrecio""
               where ""Precio""!=0 and ""CodArticulo"" = '{0}'
            ", codArticulo);
            if (!string.IsNullOrEmpty(listaPrecio)){
                sql += string.Format(@" and upper(""ListaPrecio"") like '%{0}%'", listaPrecio);
            }
            var bc = new BaseCore();
            var dt = bc.GetDataTableByQuery(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {

                    ms.Add(new PriceListMsg
                    {
                        id = dr["Id"].ToString(),
                        priceList = dr["ListaPrecio"].ToString().Replace("LISTA DE PRECIOS ", null),
                        price = bc.GetDecimal(dr["Precio"])

                    });
                }
            }
            return ms;
        }

        public static List<object> GetForMarketingVET()
        {
            var ms = new List<object>();
            var sql = string.Format(@"
                select 
                 ""CodArticulo"",
                 ""Grupo"",
                 ""Articulo"",
                 ""Estado"",
                 ""CantidadPedidoMinima"",
                 ""LeadTimeDias"",
                 ""vidaUtilMeses"",
                 ""LeadTimeManufactura"",
                 ""TamañoLote"",
                 ""VerEnApp"",
                 ""Categoria"",
                 ""FormaFarmaceutica"",
                 ""CondicionAlmacenamiento"",
                 ""EAN13"",
                 ""EAN14"",
                 ""StockNecesario"",
                 ""StockMinimo""
                from
                 ""JbVw_DetalleArticulosMarketing""
            ");
            var bc = new BaseCore();
            var dt = bc.GetDataTableByQuery(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {

                    ms.Add(new 
                    {
                        CodArticulo= dr["CodArticulo"].ToString(),
                        Grupo = dr["Grupo"].ToString(),
                        Articulo = dr["Articulo"].ToString(),
                        Estado = dr["Estado"].ToString(),
                        CantidadPedidoMinima = bc.GetInt(dr["CantidadPedidoMinima"]),
                        LeadTimeDias = bc.GetInt(dr["LeadTimeDias"]),
                        vidaUtilMeses = bc.GetInt(dr["vidaUtilMeses"]),
                        LeadTimeManufactura = bc.GetInt(dr["LeadTimeManufactura"]),
                        TamañoLote = bc.GetInt(dr["TamañoLote"]),
                        VerEnApp = dr["VerEnApp"].ToString(),
                        Categoria = dr["Categoria"].ToString(),
                        FormaFarmaceutica = dr["FormaFarmaceutica"].ToString(),
                        CondicionAlmacenamiento = dr["CondicionAlmacenamiento"].ToString(),
                        EAN13 = dr["EAN13"].ToString(),
                        EAN14 = dr["EAN14"].ToString(),
                        StockNecesario = bc.GetInt(dr["StockNecesario"]),
                        StockMinimo = bc.GetInt(dr["StockMinimo"]),
                    });
                }
            }
            return ms;
        }

        public static List<object> getListaPrecioVET()
        {
            var ms = new List<object>();
            var sql = string.Format(@"
                select 
                 ""CodArticulo"",
                 ""Articulo"",
                 ""ListaPrecio"",
                 ""Precio""
                from
                 ""JbVw_GetListaPreciosVET""
            ");
            var bc = new BaseCore();
            var dt = bc.GetDataTableByQuery(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {

                    ms.Add(new
                    {
                        CodArticulo = dr["CodArticulo"].ToString(),
                        Articulo = dr["Articulo"].ToString(),
                        ListaPrecio = dr["ListaPrecio"].ToString(),
                        Precio = bc.GetDouble(dr["Precio"]),
                    });
                }
            }
            return ms;
        }
    }
}
