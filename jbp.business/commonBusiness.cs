using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using jbp.msg;

namespace jbp.business
{
    public class commonBusiness
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
    }
}
