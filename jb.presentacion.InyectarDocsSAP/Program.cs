namespace jb.presentacion.InyectarDocsSAP
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.ThreadException += (sender, args) =>
            {
                File.AppendAllText("fatal.log",
                    $"[UI THREAD] {DateTime.Now}\n{args.Exception}\n\n");
            };

            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                var ex = args.ExceptionObject as Exception;
                File.AppendAllText("fatal.log",
                    $"[NON UI THREAD] {DateTime.Now}\n{ex}\n\n");
            };

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new frmSyncAppVET());
        }
    }
}