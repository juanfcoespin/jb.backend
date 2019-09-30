using System;
using System.IO;
using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace jbp.presentacion.organizarArchivos
{
    class Program
    {
        static void Main(string[] args)
        {
            OrganizarArchivos(InjectAppSettings());
            Show("Proceso finalizado con éxito, presione cualquier tecla para finalizar...");
            Console.ReadLine();
        }

        private static IConfigurationRoot InjectAppSettings()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            return builder.Build();

        }

        private static void OrganizarArchivos(IConfigurationRoot conf)
        {

            var pathFolder = conf["path"];
            if (!Directory.Exists(pathFolder))
                return;
            foreach (var fileNamePath in Directory.GetFiles(pathFolder)) {
                //Ejemplo de nombre del archivo: JBPH_01_001010_000054644_08042019_134023.txt
                //se extrae la cadena "08042019" correspondiente al 8 de abril del 2019
                var vector2 = fileNamePath.Split(new char[] {'\\'});
                var fileName = vector2[vector2.Length-1];
                Show("Procesando archivo: "+fileName);
                var vector = fileName.Split(new char[] {'_'});
                
                if (vector.Length > 4) {
                    var fecha = vector[4];
                    var año = fecha.Substring(4, 4);//2019
                    var mes = fecha.Substring(2, 2);
                    mes = string.Format("{0}-{1}",año,mes);
                    var dia = fecha.Substring(0, 2);//2019-04
                    dia = string.Format("{0}-{1}",mes,dia);//2019-04-08
                    var folder = pathFolder + "\\" + año;
                    CrearCarpeta(folder);
                    folder += "\\" + mes;
                    CrearCarpeta(folder);
                    folder += "\\" + dia;
                    CrearCarpeta(folder);
                    var destination = string.Format("{0}\\{1}", folder,fileName);
                    File.Move(fileNamePath, destination);
                }
            }
        }

        private static void Show(string msg)
        {
            Console.WriteLine(msg);
        }

        private static void CrearCarpeta(string folderName) {
            if (!Directory.Exists(folderName))
            {
                Directory.CreateDirectory(folderName);
                Show("Creada carpeta: " + folderName);
            }
        }
    }
}
