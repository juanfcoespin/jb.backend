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
            EnviarDocumentos();
            //RegistrarParticipante("1721796751001");
            Console.WriteLine("Fin Proceso");
        }

        private static void RegistrarParticipante(string ruc)
        {
            new ParticipantePtkBusiness().RegistrarParticipante(ruc);
        }

        private static void EnviarDocumentos()
        {
            new DocumentosPtkBusiness().EnviarDocumentosAPromotick();
        }
    }
}
