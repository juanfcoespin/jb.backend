using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbp.utils
{
    public class FechaUtils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="me"></param>
        /// <param name="format">puede ser 'yyyy/mm/dd','yyyy/mm', 'dd-mm-yyyy', 'YYYY-DD-MM'</param>
        /// <returns>string correspondiente a la fecha en el formato solicitado</returns>
        public static string getStringDate(DateTime me, string format) {
            if (me == null)
                return null;
            var ms = string.Empty;
            var separator = string.Empty;
            if (format.Contains("/"))
                separator = "/";
            if (separator==string.Empty && format.Contains("-"))
                separator = "-";
            var dateFormatMatriz = format.Split(new char[] {'/','-' });
            if (dateFormatMatriz.Length == 0)
                return null;
            foreach (var item in dateFormatMatriz) {
                var token = item.ToLower().Trim();
                if (!string.IsNullOrEmpty(ms))
                    ms += separator;
                switch (token)
                {
                    case "yyyy":
                        ms += me.Year.ToString();
                        break;
                    case "mm":
                        ms += StringUtils.getTwoDigitNumber(me.Month);
                        break;
                    case "dd":
                        ms += StringUtils.getTwoDigitNumber(me.Day);
                        break;
                }
            }
            return ms;
        }
    }
}
