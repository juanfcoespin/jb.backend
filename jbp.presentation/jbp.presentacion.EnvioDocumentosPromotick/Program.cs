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
            //Console.WriteLine("Inicio Proceso");
            //new DocumentosPtkBusiness().EnviarDocumentosAPromotick();
            new DocumentosPtkBusiness().EnviarNotasCreditoManuales();
            //new DocumentosPtkBusiness().EnviarAjustes();
            //new DocumentosPtkBusiness().EnviarAceleradores("Jul-Sep 2025");
            //new ParticipantePtkBusiness().InactivarParticipantes();
            //new ParticipantePtkBusiness().ActualizacionMasivaParticipantes();
            //new ParticipantePtkBusiness().RegistroMasivoParticipantes();
            //new BaseBusiness().testCorreo("<b>Cabecera</b><br> Esto es una prueba");
            //new ParticipantePtkBusiness().AsignacionMasivaVendedor();
            //new VendedorPtkBusiness().ActualizacionMasivaVendedores(eTipoOperacionVendedor.insert);
            //Console.WriteLine("Fin Proceso");
        }
    }
}
