﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Serilog;
namespace TX
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            string Folder = "logs";
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["logFolder"]))
                Folder = ConfigurationManager.AppSettings["logFolder"];

            Log.Logger = new LoggerConfiguration()
             .MinimumLevel.Debug()
             .WriteTo.Console()
             .WriteTo.File($"{Folder}/Txlog.txt", rollingInterval: RollingInterval.Day)
             .CreateLogger();

            if (Environment.UserInteractive)
            {
                var service = new TxService();
                service.DebugRun(); // A method you create to simulate OnStart
            }
            else
            {
                ServiceBase.Run(new TxService());
            }
        }

     
    }
}
