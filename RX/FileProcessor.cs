using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace RX
{
    public class FileProcessor:IFileHandler
    {
        private FileSystemWatcher watcher;
        private string ServiceName { get; }
        private string Source { get; }
        private MailSettings MailSettings;


        public FileProcessor(string ServiceName,string Source, MailSettings MailSettings) {
        this.ServiceName = ServiceName;
            this.Source = Source;  
            this.MailSettings = MailSettings;
        }

        public void Handle(string sourcePath, string fileName)
        {
            Log.Information("Handle()");
            EventLogger.WriteToEventLog(this.ServiceName, "Handle()", EventLogEntryType.Information);

            if (FileFormatChecker.IsFileFlagFormat(sourcePath))
            {
                WaitUntilFileIsUnlocked(sourcePath);//waiting file will be available for delete
                File.Delete(sourcePath);

                EmailSender.SendEmail(MailSettings, "sn1983@gmail.com", "Flag Delete", $"Flag file \"{sourcePath}\" was deleted");
                Log.Information("Flag Delete", $"Flag  file \"{sourcePath}\" was deleted");
                EventLogger.WriteToEventLog(this.ServiceName, $"Flag  file \"{sourcePath}\" was deleted", EventLogEntryType.Information);

            }


        }
        private void WaitUntilFileIsUnlocked(string filePath)
        {
            while (true)
            {
                try
                {
                    using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                    {
                        break; // File is unlocked
                    }
                }
                catch (IOException)
                {
                    // Still locked, wait a bit
                    System.Threading.Thread.Sleep(200);
                }
                catch (UnauthorizedAccessException)
                {
                    System.Threading.Thread.Sleep(200);
                }
            }
        }
        public void StartWatching()
        {
            Log.Information("StartWatching()");
            EventLogger.WriteToEventLog(this.ServiceName, "StartWatching()", EventLogEntryType.Information);

            watcher = new FileSystemWatcher(Source)
            {
                Filter = "*.*",
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
                EnableRaisingEvents = true
            };

            watcher.Created += (s, e) => Handle(e.FullPath, e.Name);
            watcher.Changed += (s, e) => Handle(e.FullPath, e.Name);
        }
    }
}
