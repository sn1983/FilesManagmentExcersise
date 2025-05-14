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
using ClassHelpers;
using Serilog;

namespace TX
{
    public partial class TxService : ServiceBase
    {


        TransferSettings transferSettings;
        public TxService()
        {
            InitializeComponent();
            Log.Information("Serilog is working!");

            //TODO: Read Thoes Values from DB somehow

            string TransferSettingsPath = string.Empty;
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["TransferSettingsPath"]))
                TransferSettingsPath = ConfigurationManager.AppSettings["TransferSettingsPath"];
           
            transferSettings = JsonHelper.JsonHelper.LoadFromFile<TransferSettings>(TransferSettingsPath);
            if (transferSettings == null)
            {
                //case file is empty or not exists we initalize from appConfig
                transferSettings = new TransferSettings();
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["SourceFolder"]))
                    transferSettings.SourceFolder = ConfigurationManager.AppSettings["SourceFolder"];

                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["DestinationFolder"]))
                    transferSettings.DestinationFolder = ConfigurationManager.AppSettings["DestinationFolder"];
                //TODO: create folders if not exists
                transferSettings.SysLog = true;
                JsonHelper.JsonHelper.SaveToFile<TransferSettings>(TransferSettingsPath, transferSettings);
            }
           
            LocalFileTransfer fr = new LocalFileTransfer(transferSettings.SourceFolder, transferSettings.DestinationFolder, ServiceName);
            fr.StartWatching();
        }



        protected override void OnStart(string[] args)
        {
            Log.Information("Service OnStart()");
            if(transferSettings.SysLog)
           EventLogger.WriteToEventLog(this.ServiceName, "Service OnStart()", EventLogEntryType.Information);
            FlagCreator fc = new FlagCreator(transferSettings.SourceFolder, ServiceName);
            fc.CreateFlagFile();
        }

        public void DebugRun()
        {
            Log.Information("DebugRun()");
            if (transferSettings.SysLog)
                EventLogger.WriteToEventLog(this.ServiceName, "Service DebugRun()", EventLogEntryType.Information);
            FlagCreator fc = new FlagCreator(transferSettings.SourceFolder, ServiceName);
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
