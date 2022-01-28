using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using jbp.msg;
using TechTools.Core.Hana;
using System.Data;


namespace jbp.business.hana
{
    public class ProductBusiness
    {
        public static List<ProductMsg> GetSellProducts()
        {
            var ms = new List<ProductMsg>();
            var sql = @"
               select 
                ""CodArticulo"", 
                ""Articulo""
               from ""JbpVw_Articulos""
               where ""Vendible"" = 'SI'
                and ""VerEnMovil"" = 'SI'
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
                        ms.Add(new ProductMsg
                        {
                            id = codArticulo,
                            name = dr["Articulo"].ToString(),
                            stock = GetStockByCodArticulo(codArticulo),
                            lotes= GetLotes(codArticulo),
                            prices = prices
                        });
                    }
                 
                }
            }
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
                 t0.""Cantidad"",
                 to_char(t1.""FechaFabricacion"", 'yyyy-mm-dd') ""FechaFabricacion"",
                 to_char(t1.""FechaVencimiento"", 'yyyy-mm-dd') ""FechaVencimiento""
                from
                ""JbpVw_CantidadesPorLote"" t0 inner join
                ""JbpVw_Lotes"" t1 on t1.""Id"" = t0.""IdLote""
                where
                 t0.""Cantidad"" > 0 and
                 t0.""CodArticulo"" = '{0}' and
                 t0.""CodBodega"" in ('PT2', 'PT4', 'PICK2')
                order by t1.""FechaVencimiento""
            ", codArticulo);
            var bc = new BaseCore();
            var dt = bc.GetDataTableByQuery(sql);
            foreach(DataRow dr in dt.Rows)
            {
                ms.Add(new LoteMsg()
                {
                    lote = dr["Lote"].ToString(),
                    cantidad= bc.GetInt(dr["Cantidad"]),
                    fechaFabricacion = dr["FechaFabricacion"].ToString(),
                    fechaVencimiento = dr["FechaVencimiento"].ToString(),
                });
            }
            return ms;
        }

        public static List<PriceListMsg> GetPriceListByCodArticulo(string codArticulo)
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
        public static decimal GetStockByCodArticulo(string codArticulo)
        {       
            var sql = string.Format(@"
               select
                sum(""Stock"") ""stock""
               from
                ""JbpVw_ArticulosPorBodega""
               where
                ""CodBodega"" in ('PT2', 'PT4', 'PICK2') --no se incluye PT3 y BAL3 que es de guayaquil por petición de gardenia
                and ""CodArticulo"" = '{0}'
            ", codArticulo);
            return new BaseCore().GetDecimalScalarByQuery(sql);
        }
    }
}
