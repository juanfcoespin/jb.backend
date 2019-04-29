using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbp.utils
{
    public class MoneyUtils
    {
        /// <summary>
        /// Ej.: Entrada - Salida
        /// 1,573.35 - 1573.35
        /// 1.573,35 - 1573.35
        /// 1.573.35 - 1573.35
        /// 1573.35  - 1573.35
        /// 1,573,35 - 1573.35
        /// 1573     - 1573.00
        /// 0        - 0.00   
        /// </summary>
        /// <param name="me"></param>
        /// <returns></returns>
        public static string GetMoneyFormat(object meObj) {
            var me = string.Empty;
            if (meObj.GetType() != typeof(DBNull))
                me = Convert.ToString(meObj);
            if (string.IsNullOrEmpty(me))
                return "0.00";
            var ms = string.Empty;
            var moneyMatrix = me.Split(new char[] {',','.'});
            if (moneyMatrix.Length == 1){//Ej. 1573 ó 0
                ms = string.Format("{0}.00", me);
            }
            if (moneyMatrix.Length == 2){//Ej. 1573.35 ó 1573,5
                ms = string.Format("{0}.{1}", moneyMatrix[0], moneyMatrix[1]);
            }
            if (moneyMatrix.Length == 3){//Ej. 1,573.35 ó 1.573.35 ó 1,573,35
                ms = string.Format("{0}{1}.{2}", moneyMatrix[0], moneyMatrix[1], moneyMatrix[2]);
            }
            return ms;
        }
    }
}
