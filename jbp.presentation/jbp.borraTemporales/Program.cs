using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace jbp.borraTemporales
{
    class Program
    {
        static void Main(string[] args)
        {
            BorrarTemporales();
            Console.WriteLine("Temporales borrados!");
            
        }

        private static void BorrarTemporales()
        {
            var dirTmp = Path.GetTempPath();
            var di = new DirectoryInfo(dirTmp);
            foreach (FileInfo file in di.GetFiles())
            {
                try
                {
                    file.Delete();
                }
                catch { } //error por bloqueo de archivo o permisos
            }
            DirectoryInfo directory = new DirectoryInfo(dirTmp);
            foreach (DirectoryInfo dir in directory.GetDirectories())
            {
                try
                {
                    dir.Delete(true);
                }
                catch {}
            }
        }
    }
}
