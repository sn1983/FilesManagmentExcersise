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
    /// <summary>
    /// Windows Service for transferring files from a source folder to a destination folder.
    /// Uses configuration from a JSON file or app.config and supports event logging.
    /// </summary>
    public partial class TxService : ServiceBase
    {
        /// <summary>
        /// Holds the transfer settings loaded from configuration.
        /// </summary>
        TransferSettings transferSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="TxService"/> class.
        /// Loads settings from JSON or app.config and starts file watching.
        /// </summary>
        public TxService()
        {
            InitializeComponent();
            Log.Information("Serilog is working!");

            // TODO: Read these values from DB somehow

            string TransferSettingsPath = string.Empty;
            // Getting json file path from appConfig
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["TransferSettingsPath"]))
                TransferSettingsPath = ConfigurationManager.AppSettings["TransferSettingsPath"];

            transferSettings = JsonHelper.JsonHelper.LoadFromFile<TransferSettings>(TransferSettingsPath);
            if (transferSettings == null)
            {
                // Case file is empty or not exists we initialize from appConfig
                transferSettings = new TransferSettings();
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["SourceFolder"]))
                    transferSettings.SourceFolder = ConfigurationManager.AppSettings["SourceFolder"];

                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["DestinationFolder"]))
                    transferSettings.DestinationFolder = ConfigurationManager.AppSettings["DestinationFolder"];
               
               
                transferSettings.SysLog = true;
                JsonHelper.JsonHelper.SaveToFile<TransferSettings>(TransferSettingsPath, transferSettings);
            }
            // create folders if not exists
            if (!Directory.Exists(transferSettings.SourceFolder))
                Directory.CreateDirectory(transferSettings.SourceFolder);
            LocalFileTransfer fr = new LocalFileTransfer(transferSettings.SourceFolder, transferSettings.DestinationFolder, ServiceName);
            fr.StartWatching();
        }

        /// <summary>
        /// Executes when the service starts.
        /// Logs the start event and creates a flag file in the source folder.
        /// </summary>
        /// <param name="args">Command-line arguments.</param>
        protected override void OnStart(string[] args)
        {
            Log.Information("Service OnStart()");
            if (transferSettings.SysLog)
                EventLogger.WriteToEventLog(this.ServiceName, "Service OnStart()", EventLogEntryType.Information);
            FlagCreator fc = new FlagCreator(transferSettings.SourceFolder, ServiceName);
            fc.CreateFlagFile();
        }

        /// <summary>
        /// Runs the service logic in debug mode.
        /// Logs the debug run event and creates a flag file in the source folder.
        /// </summary>
        public void DebugRun()
        {
            Log.Information("DebugRun()");
            if (transferSettings.SysLog)
                EventLogger.WriteToEventLog(this.ServiceName, "Service DebugRun()", EventLogEntryType.Information);
            FlagCreator fc = new FlagCreator(transferSettings.SourceFolder, ServiceName);
            fc.CreateFlagFile();
            Thread.Sleep(Timeout.Infinite);
        }

        /// <summary>
        /// Executes when the service stops.
        /// Logs the stop event and flushes the logger.
        /// </summary>
        protected override void OnStop()
        {
            Log.Information("OnStop()");
            Log.CloseAndFlush();
        }
    }
}
