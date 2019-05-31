using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TechTools.Msg;
using TechTools.Utils;

namespace jbp.business
{
    public class LogBusiness
    {
        public static List<LogMsg> GetLogByDate(string me) {
            return LogUtils.GetLogsByDate(me);
        }
    }
}
