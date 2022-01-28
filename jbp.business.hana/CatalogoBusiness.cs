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
    }
}
