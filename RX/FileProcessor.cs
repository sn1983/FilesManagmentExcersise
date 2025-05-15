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
    /// <summary>
    /// Processes files in a specified directory, handling flag files and sending notifications.
    /// </summary>
    public class FileProcessor : IFileHandler
    {
        private FileSystemWatcher watcher;

        /// <summary>
        /// Gets the name of the service using this file processor.
        /// </summary>
        private string ServiceName { get; }

        /// <summary>
        /// Gets the source directory to watch for files.
        /// </summary>
        private string Source { get; }

        /// <summary>
        /// Gets the mail settings used for sending notifications.
        /// </summary>
        private MailSettings MailSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileProcessor"/> class.
        /// </summary>
        /// <param name="ServiceName">The name of the service.</param>
        /// <param name="Source">The source directory to watch.</param>
        /// <param name="MailSettings">The mail settings for notifications.</param>
        public FileProcessor(string ServiceName, string Source, MailSettings MailSettings)
        {
            this.ServiceName = ServiceName;
            this.Source = Source;
            this.MailSettings = MailSettings;
        }

        /// <summary>
        /// Handles a file event by checking if the file is a flag file, deleting it, and sending a notification email.
        /// </summary>
        /// <param name="sourcePath">The full path to the file.</param>
        /// <param name="fileName">The name of the file.</param>
        public void Handle(string sourcePath, string fileName)
        {
            Log.Information("Handle()");
            EventLogger.WriteToEventLog(this.ServiceName, "Handle()", EventLogEntryType.Information);

            if (FileFormatChecker.IsFileFlagFormat(sourcePath))
            {
                WaitUntilFileIsUnlocked(sourcePath); // Wait until the file is available for delete
                File.Delete(sourcePath);

                EmailSender.SendEmail(MailSettings, "Flag Delete", $"Flag file \"{sourcePath}\" was deleted");
                Log.Information("Flag Delete", $"Flag  file \"{sourcePath}\" was deleted");
                EventLogger.WriteToEventLog(this.ServiceName, $"Flag  file \"{sourcePath}\" was deleted", EventLogEntryType.Information);
            }
        }

        /// <summary>
        /// Waits until the specified file is unlocked and available for access.
        /// </summary>
        /// <param name="filePath">The path to the file to check.</param>
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

        /// <summary>
        /// Starts watching the source directory for new or changed files and handles them as they appear.
        /// </summary>
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
