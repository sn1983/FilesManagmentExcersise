using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

namespace RX
{
    public partial class RxService : ServiceBase
    {

        public string DestinationFolder { get; private set; }
        public RxService()
        {
            InitializeComponent();
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["DestinationFolder"]))
                DestinationFolder = ConfigurationManager.AppSettings["DestinationFolder"];
        }



        protected override void OnStart(string[] args)
        {
            Log.Information("Service OnStart()");

            EventLogger.WriteToEventLog(this.ServiceName, "Service OnStart()", EventLogEntryType.Information);
            FileProcessor fileProcessor = new FileProcessor(ServiceName, DestinationFolder);
            fileProcessor.StartWatching();
        }

        protected override void OnStop()
        {
            Log.CloseAndFlush();
        }

        internal void DebugRun()
        {
            Log.Information("DebugRun()");
            EventLogger.WriteToEventLog(this.ServiceName, "Service DebugRun()", EventLogEntryType.Information);
            FileProcessor fileProcessor = new FileProcessor(ServiceName, DestinationFolder);
            fileProcessor.StartWatching();
            Thread.Sleep(Timeout.Infinite);
        }
    }
}
