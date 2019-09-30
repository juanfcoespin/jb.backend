using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using jbp.msg;
using jbp.business.services;
using System.IO;
using TechTools.DelegatesAndEnums;
using TechTools.Utils;

namespace PromotickClientService
{
    class Program
    {
        
        private static CheckFacturasToSendPtkBusinessService servicePtk;
        static void  Main(string[] args)
        {
            servicePtk = new CheckFacturasToSendPtkBusinessService();
            var opt = "0";
            while (opt != "6")//mientras no se digite opción salir
            { 
                PrintMenu();
                opt = Console.ReadLine();
                switch (opt)
                {
                    case "1":
                        servicePtk.Start();
                        break;
                    case "2":
                        servicePtk.Stop();
                        break;
                    case "3":
                        OpenLog();
                        break;
                    case "4":
                        Console.Clear();
                        break;
                    case "5":
                        PrintStadoServicio();
                        break;
                    case "6":
                        servicePtk.Stop(); //para el servicio antes de salir
                        break;
                }
            }
        }
        private static void PrintStadoServicio()
        {
            var msg = servicePtk.GetStatus();
            Console.WriteLine(msg);
        }
        private static void OpenLog()
        {
            var logFileName = LogUtils.GetLogFileName();
            if (File.Exists(logFileName))
                FileUtils.Ejecutar(logFileName);
            else
                Console.Write("No se han generado logs en este día");
        }
        private static void PrintMenu()
        {
            var menu = @"
Seleccione una opcion: 
1. Iniciar servicio
2. Parar servicio
3. Ver logs de hoy
4. Limpiar pantalla
5. Consultar Estado del Servicio
6. Salir
            
Opcion: ";
            Console.WriteLine(menu);
        }
    }
}
