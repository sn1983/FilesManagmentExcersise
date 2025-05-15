using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ClassHelpers;
using JsonHelper;
using Serilog;

namespace RX
{
    /// <summary>
    /// Windows Service for receiving files and processing them.
    /// Loads configuration from JSON or app.config, initializes mail settings, and sets up logging.
    /// </summary>
    public partial class RxService : ServiceBase
    {
        /// <summary>
        /// Gets the destination folder for received files.
        /// </summary>
        public string DestinationFolder { get; private set; }

        /// <summary>
        /// Gets or sets the mail settings used for notifications.
        /// </summary>
        public MailSettings MailSettings { get; set; }

        /// <summary>
        /// Gets or sets the RX service settings loaded from configuration.
        /// </summary>
        public RxSettings rxSettings { get; set; } = new RxSettings();

        /// <summary>
        /// Initializes a new instance of the <see cref="RxService"/> class.
        /// Loads settings from JSON or app.config, ensures destination folder exists, and initializes logging.
        /// </summary>
        public RxService()
        {
            InitializeComponent();
            //getting this data from external source
            string rxSettingsPath = string.Empty;
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["rxSettingsPath"]))
                rxSettingsPath = ConfigurationManager.AppSettings["rxSettingsPath"];
            rxSettings = JsonHelper.JsonHelper.LoadFromFile<RxSettings>(rxSettingsPath);
            if (rxSettings == null)//means definition file is not exists
            {
                rxSettings = new RxSettings();
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["DestinationFolder"]))
                    rxSettings.DestinationFolder = ConfigurationManager.AppSettings["DestinationFolder"];
                rxSettings.SysLog = true;

                rxSettings.SmtpServer = string.IsNullOrEmpty(ConfigurationManager.AppSettings["SmtpServer"]) ? string.Empty : ConfigurationManager.AppSettings["SmtpServer"];
                rxSettings.Port = string.IsNullOrEmpty(ConfigurationManager.AppSettings["Port"]) ? -1 : int.Parse(ConfigurationManager.AppSettings["Port"]);
                rxSettings.UseSsl = string.IsNullOrEmpty(ConfigurationManager.AppSettings["UseSsl"]) ? false : bool.Parse(ConfigurationManager.AppSettings["UseSsl"]);
                rxSettings.SenderEmail = string.IsNullOrEmpty(ConfigurationManager.AppSettings["SenderEmail"]) ? string.Empty : ConfigurationManager.AppSettings["SenderEmail"];
                rxSettings.SenderName = string.IsNullOrEmpty(ConfigurationManager.AppSettings["SenderName"]) ? string.Empty : ConfigurationManager.AppSettings["SenderName"];
                rxSettings.Username = string.IsNullOrEmpty(ConfigurationManager.AppSettings["Username"]) ? string.Empty : ConfigurationManager.AppSettings["Username"];
                rxSettings.Password = string.IsNullOrEmpty(ConfigurationManager.AppSettings["Password"]) ? string.Empty : ConfigurationManager.AppSettings["Password"];
                rxSettings.logFolder = string.IsNullOrEmpty(ConfigurationManager.AppSettings["logFolder"]) ? string.Empty : ConfigurationManager.AppSettings["logFolder"];
                rxSettings.RecpientEmail = string.IsNullOrEmpty(ConfigurationManager.AppSettings["RecpientEmail"]) ? string.Empty : ConfigurationManager.AppSettings["recpientEmail"];

                JsonHelper.JsonHelper.SaveToFile<RxSettings>(rxSettingsPath, rxSettings);
            }
            if (!Directory.Exists(rxSettings.DestinationFolder))
                Directory.CreateDirectory(rxSettings.DestinationFolder);

            MailSettings = new MailSettings()
            {
                SmtpServer = rxSettings.SmtpServer,
                Port = rxSettings.Port,
                UseSsl = rxSettings.UseSsl,
                SenderEmail = rxSettings.SenderEmail,
                SenderName = rxSettings.SenderName,
                Username = rxSettings.Username,
                Password = rxSettings.Password,
                RecpientEmail = rxSettings.RecpientEmail    
            };
            InitializeLogger();
        }

        /// <summary>
        /// Executes when the service starts.
        /// Logs the start event and starts the file processor.
        /// </summary>
        /// <param name="args">Command-line arguments.</param>
        protected override void OnStart(string[] args)
        {
            Log.Information("Service OnStart()");
            EventLogger.WriteToEventLog(this.ServiceName, "Service DebugRun()", EventLogEntryType.Information);
            FileProcessor fileProcessor = new FileProcessor(ServiceName, DestinationFolder, MailSettings);
            fileProcessor.StartWatching();
        }

        /// <summary>
        /// Executes when the service stops.
        /// Flushes and closes the logger.
        /// </summary>
        protected override void OnStop()
        {
            Log.CloseAndFlush();
        }

        /// <summary>
        /// Runs the service logic in debug mode.
        /// Logs the debug run event and starts the file processor.
        /// </summary>
        internal void DebugRun()
        {
            Log.Information("DebugRun()");
            EventLogger.WriteToEventLog(this.ServiceName, "Service DebugRun()", EventLogEntryType.Information);
            FileProcessor fileProcessor = new FileProcessor(ServiceName, DestinationFolder, MailSettings);
            fileProcessor.StartWatching();
            Thread.Sleep(Timeout.Infinite);
        }

        /// <summary>
        /// Initializes the Serilog logger using the log folder from settings.
        /// </summary>
        private void InitializeLogger()
        {
            // Set the default log folder
            string Folder = "logs";
            Folder = rxSettings.logFolder;

            Log.Logger = new LoggerConfiguration()
             .MinimumLevel.Debug()
             .WriteTo.Console()
             .WriteTo.File($"{Folder}/Rxlog.txt", rollingInterval: RollingInterval.Day)
             .CreateLogger();
        }
    }
}
