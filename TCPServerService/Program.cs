using System.ServiceProcess;

namespace TCPServerService
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main()
        {
#if !DEBUG
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                new Service()
                };
                ServiceBase.Run(ServicesToRun);
            }
#else
            {
                Service DebugService = new Service();
                DebugService.OnDebug();
            }
#endif
        }
    }
}
