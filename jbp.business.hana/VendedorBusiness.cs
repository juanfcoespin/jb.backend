using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using jbp.msg;
using jbp.msg.sap;
using TechTools.Core.Hana;
using System.Data;

namespace jbp.business.hana
{
    public class VendedorBusiness
    {
        public static List<CarteraMsg> GetCarteraByCodVendedor(string codVendedor)
        {
            var ms = new List<CarteraMsg>();
            var sql = @"
              select
                 t0.""DocNum"",
                 t1.""CodSocioNegocio"", 
                 t0.""TipoDocumento"",
                 t0.""Vendedor"",
                 t0.""NumFolio"",
                 to_char(t0.""FechaDocumento"", 'yyyy-mm-dd') ""FechaDocumento"",
                 t0.""TotalDocumento"",
                 to_char(t0.""FechaVencimiento"", 'yyyy-mm-dd') ""FechaVencimiento"",
                 t0.""DiasVencido"",
                 t0.""TotalPago"",
                 t0.""SaldoVencido"",
                 t0.""RangoDiasVencido"",
                 t0.""OrdenRango"",
                 t0.""OrdenTipoDocumento""
                from ""JbpVw_Cartera"" t0  inner join
                ""JbpVw_SocioNegocio"" t1 on t1.""CodSocioNegocio"" = t0.""CodCliente""
            ";
            if (!string.IsNullOrEmpty(codVendedor) && codVendedor != "0") {// si no es administrador (es vendedor)
                sql += string.Format(@"
                where
                 t1.""CodVendedor"" = {0}
                ",codVendedor
                );
            }
            sql +=@"
                order by
                 t1.""CodSocioNegocio"",
                 t0.""OrdenRango""
            "; 
            var bc = new BaseCore();
            var dt = bc.GetDataTableByQuery(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    ms.Add(new CarteraMsg
                    {
                        DocNum = bc.GetInt(dr["DocNum"]),
                        CodSocioNegocio = dr["CodSocioNegocio"].ToString(),
                        TipoDocumento = dr["TipoDocumento"].ToString(),
                        Vendedor = dr["Vendedor"].ToString(),
                        NumFolio = dr["NumFolio"].ToString(),
                        FechaDocumento = dr["FechaDocumento"].ToString(),
                        TotalDocumento = bc.GetDecimal(dr["TotalDocumento"]),
                        FechaVencimiento = dr["FechaVencimiento"].ToString(),
                        DiasVencido = bc.GetInt(dr["DiasVencido"]),
                        TotalPago = bc.GetDecimal(dr["TotalPago"]),
                        SaldoVencido = bc.GetDecimal(dr["SaldoVencido"]),
                        RangoDiasVencido = dr["RangoDiasVencido"].ToString(),
                        OrdenRango= bc.GetInt(dr["OrdenRango"]),
                        OrdenTipoDocumento = bc.GetInt(dr["OrdenTipoDocumento"])
                        
                    });
                }
            }
            //se agregan retenciones a las facturas
            ms.ForEach(fact =>
            {
                if (fact.TipoDocumento == "Factura") {
                    fact.Retenciones = FacturaBusiness.GetRetencionesByDocNum(fact.DocNum);
                }
            });
            return ms;
        }

        public static List<ItemCombo> GetList()
        {
            var ms = new List<ItemCombo>();
            var sql = @"
                select 
                 ""CodVendedor"",
                 ""Cedula"",
                 ""Vendedor""
                from ""JbpVw_Vendedores""
                where ""SincronizadoPtk"" = 'SI'
                order by 3
            ";
            var bc = new BaseCore();
            var dt = bc.GetDataTableByQuery(sql);
            foreach (DataRow dr in dt.Rows) {
                ms.Add(new ItemCombo { 
                    Id=bc.GetInt(dr["CodVendedor"]),
                    Cod=dr["Cedula"].ToString(),
                    Nombre = dr["Vendedor"].ToString(),
                });
            }
            return ms;
        }
    }
}
