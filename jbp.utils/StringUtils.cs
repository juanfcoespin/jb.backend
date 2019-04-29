using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbp.utils
{
    public class StringUtils
    {
        public static string getTwoDigitNumber(int me) {
            if (me < 10)
                return "0" + me.ToString();
            return me.ToString();
        }
    }
}
