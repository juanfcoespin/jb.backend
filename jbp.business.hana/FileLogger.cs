using System;
using System.IO;

namespace jbp.business.hana
{
    public static class FileLogger
    {
        public static void WriteLogToFile(string level, string message, string proceso)
        {
            try
            {
                string logDirectory = @"\\192.168.57.115\jbapps\services.jbp.com.ec\logs\" + proceso;
                if (!Directory.Exists(logDirectory))
                    Directory.CreateDirectory(logDirectory);

                string fileName = $"{DateTime.Now:yyyy-MM}.txt";
                string filePath = Path.Combine(logDirectory, fileName);

                // Formato estándar que facilita la integración
                string logLine = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {message}";

                File.AppendAllText(filePath, logLine + Environment.NewLine);
            }
            catch
            {
                // Ignorar error al escribir log en disco para no detener el flujo
            }
        }
    }
}
