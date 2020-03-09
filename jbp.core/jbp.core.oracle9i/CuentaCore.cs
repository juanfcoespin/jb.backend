using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using jbp.msg;
using jbp.utils;
using System.Data;
using TechTools.Utils;

namespace jbp.core
{
    public class CuentaCore:BaseCore
    {
        public void DeletePlanCuentas(PlanCuentasProcesadoMsg me)
        {
            var sql = string.Format(@"
            delete from gms.TBL_PLAN_CTAS_PROCESADO
            where 
                anio ='{0}'
                and mes='{1}'
                and cuenta='{2}'
            ", 
            me.Periodo.FechaInicio.Year,
            StringUtils.getTwoDigitNumber(me.Periodo.FechaInicio.Month),
            me.Cuenta);
            Execute(sql);
        }
        public bool ExistPlanCuentas(PlanCuentasProcesadoMsg me)
        {
            var sql = string.Format(@"
            select count(*) from gms.TBL_PLAN_CTAS_PROCESADO
            where 
                anio ='{0}'
                and mes='{1}'
                and cuenta='{2}'
            ", me.Periodo.FechaInicio.Year,
            StringUtils.getTwoDigitNumber(me.Periodo.FechaInicio.Month),
            me.Cuenta);
            return GetIntScalarByQuery(sql) > 0;
        }

        public ListMS<CuentaMsg> GetList()
        {
            var ms = new ListMS<CuentaMsg>();
            var sql = @"
             SELECT
              CUENTA, NIVEL_1, NIVEL_2, NIVEL_3, NIVEL_4, NIVEL_5,
              CTA_NOMBRE
             FROM
              GMS.TBL_PLAN_CTAS_TODAS";
            var dt = GetDataTableByQuery(sql);
            foreach (DataRow dr in dt.Rows) {
                ms.List.Add(
                    new CuentaMsg {
                        Cuenta = dr["CUENTA"].ToString(),
                        Nivel1 = dr["NIVEL_1"].ToString(),
                        Nivel2 = dr["NIVEL_2"].ToString(),
                        Nivel3 = dr["NIVEL_3"].ToString(),
                        Nivel4 = dr["NIVEL_4"].ToString(),
                        Nivel5 = dr["NIVEL_5"].ToString(),
                        CuentaNombre = dr["CTA_NOMBRE"].ToString(),
                    }
                );
            }
            return ms;
        }
        public void Insert(PlanCuentasProcesadoMsg me)
        {
            var sql = string.Format(@"
                INSERT INTO GMS.TBL_PLAN_CTAS_PROCESADO(
                    ANIO,MES,NIVEL1,NIVEL2,NIVEL3,NIVEL4,NIVEL5,CUENTA, REAL,PRESUP,PCODE, CTA_NOMBRE
                )values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}',{8},{9},{10},'{11}')",
                    me.Periodo.FechaInicio.Year,
                    StringUtils.getTwoDigitNumber(me.Periodo.FechaInicio.Month),
                    me.Nivel1, me.Nivel2, me.Nivel3, me.Nivel4, me.Nivel5,
                    me.Cuenta, MoneyUtils.GetMoneyFormat(me.Real),
                    me.Cuenta, MoneyUtils.GetMoneyFormat(me.Presupuesto),
                    me.Periodo, me.CuentaNombre
                );
            Execute(sql);
        }
    }
}
