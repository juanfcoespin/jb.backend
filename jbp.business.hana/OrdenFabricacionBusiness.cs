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
        public static List<string> GetOfLiberadas()
        {
            var ms = new List<string>();
            var sql = @"
                select ""DocNum"" from ""JbpVw_OrdenFabricacion""
                where ""Estado"" = 'Liberado'
            ";
            var dt = new BaseCore().GetDataTableByQuery(sql);
            foreach (DataRow dr in dt.Rows) {
                ms.Add(dr["DocNum"].ToString());
            }
            return ms;
        }

        public static List<OrdenFabricacionMsg> GetComponentesOfByDocNum(int docNum)
        {
            var ms = new List<OrdenFabricacionMsg>();
            var sql = string.Format( @"
                select 
                 t2.""CodInsumo"",
                 t2.""UnidadMedida"",
                 t2.""Insumo"",
                 t0.""CantidadPlanificada"" 
                from 
                 ""JbpVw_OrdenFabricacionLinea"" t0 inner join
                 ""JbpVw_OrdenFabricacion"" t1 on t1.""Id""=t0.""IdOrdenFabricacion"" inner join
                 ""JbpVw_Insumos"" t2 on t2.""CodInsumo""=t0.""CodInsumo""
                where
                 t1.""DocNum""={0}
                 and t2.""TipoInsumo""='Artículo'

            ",docNum);
            var bc = new BaseCore();
            var dt = bc.GetDataTableByQuery(sql);
            foreach (DataRow dr in dt.Rows)
            {
                ms.Add(new OrdenFabricacionMsg { 
                    Codigo = dr["CodInsumo"].ToString(),
                    UnidadMedida = dr["UnidadMedida"].ToString(),
                    Descripcion = dr["Insumo"].ToString(),
                    Cantidad = bc.GetDecimal(dr["CantidadPlanificada"])
                });
            }
            return ms;
        }
    }
}
