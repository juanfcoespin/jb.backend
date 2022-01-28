using jbp.msg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTools.Core.Hana;
using System.Data;


namespace jbp.business.hana
{
    public class EntregaBusiness
    {
        public static List<EntregaUrbanoMS> GetEntregasUrbano(EntregaUrbanoME me)
        {
            //2021-06-04T20:58:04.373Z
            me.fechaDesde = me.fechaDesde.Substring(0, 10);
            me.fechaHasta = me.fechaHasta.Substring(0, 10);
            var ms = new List<EntregaUrbanoMS>();
            var sql = string.Format(@"
                call ""JbpSp_EntregasUrbano""('{0}','{1}','*','*','*', '{2}')",
                me.fechaDesde, me.fechaHasta, me.bodega);
            var bc = new BaseCore();
            var dt = bc.GetDataTableByQuery(sql);
            foreach(DataRow dr in dt.Rows)
            {
                ms.Add(new EntregaUrbanoMS
                {
                    DocNum = dr["DocNum"].ToString(),
                    NumFactura = dr["NumFactura"].ToString(),
                    Bodega = dr["Bodega"].ToString(),
                    Fecha = dr["Fecha"].ToString(),
                    CantBultos = bc.GetInt(dr["CantBultos"]),
                    Cliente = dr["NombreCliente"].ToString(),
                    Cedula = dr["Cedula / Codigo Cliente"].ToString(),
                });
            }
            return ms;
        }

        public static List<EntregaHojaRutaMS> GetEntregasHojaRuta(EntregaHojaRutaME me)
        {
            var bodegas = new List<string>();
            switch (me.lugar) {
                case "PIFO":
                    bodegas.Add("PT1");
                    break;
                case "PUEMBO":
                    bodegas.Add("PICK2");
                    break;
            }
            //2021-06-04T20:58:04.373Z
            me.fechaDesde = me.fechaDesde.Substring(0, 10);
            me.fechaHasta = me.fechaHasta.Substring(0, 10);
            var ms = new List<EntregaHojaRutaMS>();
            //en la vista se ordena por nro de factura
            var sql = string.Format(@"
                select 
                 ""NombreCliente"",                 
                 ""NumeroFactura"",
                 ""Fecha"",
                 ""Bodega"",
                 ""CantBultos"",
                 ""Transporte"",
                 ""Ciudad"",
                 ""Comentarios"",
                 ""NumeroGuia"",
                 ""MargenSuperior"",
                 ""MargenIzquierdo"",
                 ""FechaImpresion""
                from 
                 ""JbpVw_HojaDeRuta""
                where 
                 ""Fecha"" between to_date('{0}','yyyy-mm-dd') 
                 and to_date('{1}', 'yyyy-mm-dd')
                ",
                me.fechaDesde, me.fechaHasta);
            var bc = new BaseCore();
            var dt = bc.GetDataTableByQuery(sql);
            foreach (DataRow dr in dt.Rows)
            {
                var bodega = dr["Bodega"].ToString();
                //al poner como where en el query para filtrar por bodega da un error raro
                if (bodegas.Contains(bodega)){
                    ms.Add(new EntregaHojaRutaMS
                    {
                        Cliente = dr["NombreCliente"].ToString(),
                        NumFactura = dr["NumeroFactura"].ToString(),
                        Fecha = dr["Fecha"].ToString(),
                        Bodega = bodega,
                        CantBultos = dr["CantBultos"].ToString(),
                        Transporte = dr["Transporte"].ToString(),
                        Ciudad = dr["Ciudad"].ToString(),
                        Observaciones = dr["Comentarios"].ToString(),
                        NumeroGuia = dr["NumeroGuia"].ToString(),
                        MargenSuperior = dr["MargenSuperior"].ToString(),
                        MargenIzquierdo = dr["MargenIzquierdo"].ToString(),
                        FechaImpresion = dr["FechaImpresion"].ToString()
                    });
                }
                
            }
            return ms;
        }
    }
}
