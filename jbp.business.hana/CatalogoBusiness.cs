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
    public class CatalogoBusiness
    {
        public static CatalogoPesajeMsg GetCatalogoPesaje()
        {
            var ms = new CatalogoPesajeMsg { 
                materiasPrimas = ProductBusiness.GetArticulosByPatronCodigo("'105%', '106%', '110%', '111%'"),
                productosTerminados = ProductBusiness.GetArticulosByPatronCodigo("'8%'")
            };
            return ms;
        }

        internal static List<ItemCatalogo> GetCatalogByName(string catalogName)
        {
            var ms=new List<ItemCatalogo>();
            var sql = string.Format(@"
                SELECT 
                 T0.ID,
                 T0.VALUE
                FROM 
                 JB_CATALOG_VALUES T0 inner join
                 JB_CATALOG T1 ON T1.ID=T0.ID_CATALOG
                WHERE
                 T1.NAME='{0}'
            ",catalogName);
            var bc = new BaseCore();
            var dt=bc.GetDataTableByQuery(sql);
            foreach (DataRow dr in dt.Rows) {
                ms.Add(new ItemCatalogo { 
                    id = bc.GetInt(dr["ID"]),
                    name = dr["VALUE"].ToString()
                });
            }
            return ms;
        }
    }
}
