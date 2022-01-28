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
    public class TransportistaBusiness
    {
        public static List<ItemCombo> GetTransportistas()
        {
            var ms = new List<ItemCombo>();
            var sql = @"
               select 
                ""CodTransportista"", 
                ""Transportista""
               from
                ""JbpVw_Transportista""
               order by 2
                
            ";
            var bc = new BaseCore();
            var dt = bc.GetDataTableByQuery(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    ms.Add(new ItemCombo
                    {
                        Id = bc.GetInt(dr["CodTransportista"]),
                        Nombre = dr["Transportista"].ToString()
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
                 t0.""CodBodega"" in ('PT2', 'PT4')
                order by t1.""FechaVencimiento""
            ",codArticulo);
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
                ""CodBodega"" in ('PT2', 'PT4') --no se incluye PT3 y BAL3 que es de guayaquil por petición de gardenia
                and ""CodArticulo"" = '{0}'
            ", codArticulo);
            return new BaseCore().GetDecimalScalarByQuery(sql);
        }
    }
}
