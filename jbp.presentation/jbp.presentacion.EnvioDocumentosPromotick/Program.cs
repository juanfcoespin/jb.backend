using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using jbp.business.hana;
using jbp.msg;

namespace jbp.presentacion.EnvioDocumentosPromotick
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Inicio Proceso");
            new DocumentosPtkBusiness().EnviarDocumentosAPromotick();
            Console.WriteLine("Fin Proceso");
        }
    }
}
