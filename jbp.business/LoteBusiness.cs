using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using jbp.core;
using TechTools.Exceptions;
using System.Data;

namespace jbp.business
{
    public class LoteBusiness
    {
        public static string GetLoteByOrdenIdRecursoLineaOrden(string orden, string idRecurso, string lineaOrden) {
            try
            {
                var ms = "";
                var sql = string.Format(@"
                     Select 
                            DISTINCT B.Lot LOTE 
                        From
                            inShippingLineI B 
                        Where
                            B.LOGDOCNUM = '{0}'
                            And B.RESCOBJECTID = {1}
                            AND B.LOGDOCLINENUM = {2}
                ",orden, idRecurso,lineaOrden);
                var dt = new BaseCore().GetDataTableByQuery(sql);
                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows) {
                        ms += dr["Lote"].ToString();
                    }
                }
                return ms;
            }
            catch (Exception e)
            {
                e = ExceptionManager.GetDeepErrorMessage(e, ExceptionManager.eCapa.Business);
                throw e;
            }
        }
    }
}
