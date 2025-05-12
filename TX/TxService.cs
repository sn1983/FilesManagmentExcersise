using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Serilog;

namespace TX
{
    public partial class TxService : ServiceBase
    {

        public string Folder { get; set; }
        public string DestinationFolder { get; set; }
        public TxService()
        {
            InitializeComponent();
            Log.Information("Serilog is working!");

            //TODO: Read Thoes Values from DB somehow
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["SourceFolder"]))
                Folder = ConfigurationManager.AppSettings["SourceFolder"];

            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["DestinationFolder"]))
                DestinationFolder = ConfigurationManager.AppSettings["DestinationFolder"];

            LocalFileTransfer fr = new LocalFileTransfer(Folder, DestinationFolder,ServiceName);
            fr.StartWatching();
        }



        protected override void OnStart(string[] args)
        {
            Log.Information("Service OnStart()");
           EventLogger.WriteToEventLog(this.ServiceName, "Service OnStart()", EventLogEntryType.Information);
            FlagCreator fc = new FlagCreator(Folder, ServiceName);
            fc.CreateFlagFile();
        }

        public void DebugRun()
        {
            Log.Information("DebugRun()");
            EventLogger.WriteToEventLog(this.ServiceName, "Service DebugRun()", EventLogEntryType.Information);
            FlagCreator fc = new FlagCreator(Folder, ServiceName);
            fc.CreateFlagFile();
            Thread.Sleep(Timeout.Infinite);


        }


        protected override void OnStop()
        {
            Log.Information("OnStop()");
            Log.CloseAndFlush();
        }
    }
}
