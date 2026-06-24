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
                TechTools.Utils.Logger.Error("UI THREAD EXCEPTION");
                TechTools.Utils.Logger.Error(args.Exception);
            };

            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                TechTools.Utils.Logger.Error("FATAL UNHANDLED EXCEPTION");
                TechTools.Utils.Logger.Error(args.ExceptionObject.ToString());
            };

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new frmSyncAppVET());
        }
    }
}