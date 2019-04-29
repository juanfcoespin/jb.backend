using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using utilities;

namespace jbp.utils
{
    public class LogUtils
    {
        public static void AddLog(string msg) {
            try
            {
                var logFileName = String.Format(@"{0}\logs\{1}\Log_{2}.txt",
                                    conf.Default.logsPathFolder,
                                    FechaUtils.getStringDate(DateTime.Now, "yyyy-mm"),
                                    FechaUtils.getStringDate(DateTime.Now, "yyyy-mm-dd")
                      );
                Archivo.AddDataToFile(logFileName, DateTime.Now.ToString()+": "+ msg + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al escribir en el log para escritura: {0}", ex.Message);
            }
        }
        public static void AddError(string msg) {
            AddLog("******************** Error ******************");
            AddLog(msg);
            Show(msg);
        }
        public static void AddLogAndShow(string msg) {
            AddLog(msg);
            Console.WriteLine(msg);
        }
        public static void Show(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}
