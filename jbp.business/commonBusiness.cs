using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using jbp.msg;
using jbp.core;

namespace jbp.business
{
    public class CommonBusiness
    {
        /// <summary>
        /// Aqui se almacena en memoria una lista de procesos de ejecución
        /// larga con la finalidad de notificar al cliente de software el avance 
        /// del proceso.
        /// </summary>
        public static List<ProcessCheckedMsg> ListProcessChecked =
            new List<ProcessCheckedMsg>();
        /// <summary>
        /// Trae el avance de procesamiento del proceso en porcentaje
        /// </summary>
        /// <param name="id">identificador del proceso</param>
        /// <returns></returns>
        public static int GetAdvanceProcessById(int id) {
            var process= ListProcessChecked.FirstOrDefault(p => p.Id == id);
            if (process == null)
                return 0;
            return (process.Current * 100) / process.Total;
        }
        public static bool FunGrabarTemp(double DblObjectId, string StrDoc) {
            var sql = string.Format(@"
                INSERT INTO GMS.TBL_FE_NUM_DOC VALUES({0},'{1}',TO_DATE('{2}','dd/MM/yyyy hh24:mi:ss'))",
                DblObjectId, StrDoc,DateTime.Today.Date
            );
            try
            {
                new BaseCore().Execute(sql);
                return true;
            }
            catch { return false; }
        }
        public static void DeleteTempDocFacturaElectronica(object idDocumento)
        {
            //ELIMINO DE LA TABLA TEMPORAL
            var sql = string.Format("DELETE FROM GMS.TBL_FE_NUM_DOC WHERE OBJECTID = {0}", idDocumento);
            new BaseCore().Execute(sql);
        }
    }
}
