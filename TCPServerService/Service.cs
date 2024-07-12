using System;
using System.IO;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;

namespace TCPServerService
{
    public partial class Service : ServiceBase
    {
        private static readonly string appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private Thread thread = null;
        private bool running = false;

        protected override void OnStart(string[] args)
        {
            running = true;
            Thread.CurrentThread.Name = "MainThread";

            if (!Directory.Exists(appPath + "\\Logs\\"))
            {
                _ = Directory.CreateDirectory(appPath + "\\Logs\\");
            }
            ThreadStart job = new ThreadStart(starter);
            thread = new Thread(job)
            {
                CurrentCulture = new System.Globalization.CultureInfo("en-US"),
                Name = "TCPServerService"
            };
            thread.Start();
            Logger.WriteDebugLog("Service has started.");
        }

        public void starter()
        {
            try
            {
                while (running)
                {
                    //Create a TCP/IP Server Socket
                    TCPServer server = new TCPServer();
                    server.Start();
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(ex.Message);
            }
        }

        protected override void OnStop()
        {
            running = false;
            thread.Abort();
            Thread.CurrentThread.Name = "MainThread";
            Logger.WriteDebugLog("Service has stopped.");
        }

        internal void OnDebug()
        {
            OnStart(null);
        }
    }
}