using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using jbp.msg;
using jbp.proxy;
namespace test
{
    class Program
    {
        static void  Main(string[] args)
        {
            TestWsTrandina();
        }

        private static void TestWsTrandina()
        {
            var fp = new FacturaProxy();
            fp.ShowBackgrounMessageEvent += Fp_ShowBackgrounMessageEvent;
            fp.ShowErrorMessageEvent += Fp_ShowBackgrounMessageEvent;
            fp.InsertPullFacturaTrandina();
            Console.ReadLine();
        }

        private static void Fp_ShowBackgrounMessageEvent(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}
