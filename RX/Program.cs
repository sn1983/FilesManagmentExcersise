using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Configuration;
using System.Threading.Tasks;
using Serilog;
namespace RX
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            

            if (Environment.UserInteractive)
            {
                var service = new RxService();
                service.DebugRun(); // A method you create to simulate OnStart
            }
            else
            {
                ServiceBase.Run(new RxService());
            }
        }
    }
}
