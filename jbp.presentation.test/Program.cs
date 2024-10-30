using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTools.Net;

namespace jbp.presentation.test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            enviarCorreo();
        }

        private static void enviarCorreo()
        {
            var to = "jespin@jbp.com.ec; juanfco.espin@gmail.com";
            var body = "<b>Hola</b><br>Test";
            var files = new List<string>();
            files.Add(@"C:\Users\Juan Espin\OneDrive - JAMES BROWN PHARMA\Imágenes\Capturas de pantalla\Captura de pantalla 2024-09-23 101917.png");
            files.Add(@"C:\Users\Juan Espin\Downloads\1209202401179046285400120010500000000231234567818.pdf");
            files.Add(@"c:\tmp\comprobantesPago\C1104503964001_202409231237_1.png");
            var resp=MailUtils.Send(to, "Test Correo", body, files);
            Console.WriteLine(resp);
        }
    }
}
