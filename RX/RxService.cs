using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
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
    public partial class RxService : ServiceBase
    {

        public string DestinationFolder { get; private set; }
        public MailSettings MailSettings { get; set; }

        public RxSettings rxSettings { get; set; } = new RxSettings();
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
                rxSettings.Port = string.IsNullOrEmpty(ConfigurationManager.AppSettings["Port"]) ? -1 : int.Parse(ConfigurationManager.AppSettings["Port"]); ;
                rxSettings.UseSsl = string.IsNullOrEmpty(ConfigurationManager.AppSettings["UseSsl"]) ? false : bool.Parse(ConfigurationManager.AppSettings["UseSsl"]); ;
                rxSettings.SenderEmail = string.IsNullOrEmpty(ConfigurationManager.AppSettings["SenderEmail"]) ? string.Empty : ConfigurationManager.AppSettings["SenderEmail"]; ;
                rxSettings.SenderName = string.IsNullOrEmpty(ConfigurationManager.AppSettings["SenderName"]) ? string.Empty : ConfigurationManager.AppSettings["SenderName"]; ;
                rxSettings.Username = string.IsNullOrEmpty(ConfigurationManager.AppSettings["Username"]) ? string.Empty : ConfigurationManager.AppSettings["Username"]; ;
                rxSettings.Password = string.IsNullOrEmpty(ConfigurationManager.AppSettings["Password"]) ? string.Empty : ConfigurationManager.AppSettings["Password"]; ;
                rxSettings.logFolder = string.IsNullOrEmpty(ConfigurationManager.AppSettings["logFolder"]) ? string.Empty : ConfigurationManager.AppSettings["logFolder"]; ;
                JsonHelper.JsonHelper.SaveToFile<RxSettings>(rxSettingsPath, rxSettings);
            }

            MailSettings = new MailSettings()
            {
                SmtpServer = rxSettings.SmtpServer,
                Port = rxSettings.Port,
                UseSsl = rxSettings.UseSsl,
                SenderEmail = rxSettings.SenderEmail,
                SenderName = rxSettings.SenderName,
                Username = rxSettings.Username,
                Password = rxSettings.Password
            };
            InitializeLogger();
        }



        protected override void OnStart(string[] args)
        {
            Log.Information("Service OnStart()");
            EventLogger.WriteToEventLog(this.ServiceName, "Service DebugRun()", EventLogEntryType.Information);
            FileProcessor fileProcessor = new FileProcessor(ServiceName, DestinationFolder, MailSettings);
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
            FileProcessor fileProcessor = new FileProcessor(ServiceName, DestinationFolder, MailSettings);
            fileProcessor.StartWatching();
            Thread.Sleep(Timeout.Infinite);
        }
        private void InitializeLogger()
        {
            // Set the default log folder
            
            string Folder = "logs";
            //if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["logFolder"]))
            Folder = rxSettings.logFolder;// ConfigurationManager.AppSettings["logFolder"];

            Log.Logger = new LoggerConfiguration()
             .MinimumLevel.Debug()
             .WriteTo.Console()
             .WriteTo.File($"{Folder}/Rxlog.txt", rollingInterval: RollingInterval.Day)
             .CreateLogger();
        }
    }
}
