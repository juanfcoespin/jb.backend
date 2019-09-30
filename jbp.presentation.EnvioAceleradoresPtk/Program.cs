using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using jbp.business.promotick;
using jbp.business.contracts;

namespace jbp.presentation.EnvioAceleradoresPtk
{
    class Program
    {
        static void Main(string[] args)
        {
            var c = new ConsumoWsPtkBusiness();
            c.LogNotificationEvent += (tipo, msg) => Console.WriteLine(string.Format("{0}: {1}",tipo,msg));
            c.SendAceleradoresToWsAsync();
        }
    }
}
