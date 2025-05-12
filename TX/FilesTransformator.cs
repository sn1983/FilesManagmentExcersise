using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
namespace TX
{
    using System;
    using System.Diagnostics;
    using System.IO;

    public class LocalFileTransfer : IFileTransfer
    {
        private readonly string source;
        private readonly string destination;
        private FileSystemWatcher watcher;
        private string ServiceName { get; }
        public LocalFileTransfer(string sourceFolder, string destinationFolder, string ServiceName)
        {
            source = sourceFolder;
            destination = destinationFolder;
            ServiceName= ServiceName ?? string.Empty;
        }

        

        public void StartWatching()
        {
            Log.Information("StartWatching()");
            EventLogger.WriteToEventLog(this.ServiceName, "StartWatching()", EventLogEntryType.Information);

            watcher = new FileSystemWatcher(source)
            {
                Filter = "*.*",
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
                EnableRaisingEvents = true
            };

            watcher.Created += (s, e) => Transfer(e.FullPath, e.Name);
            watcher.Changed += (s, e) => Transfer(e.FullPath, e.Name);
        }

        public void Transfer(string sourcePath, string fileName)
        {
            Log.Information("Transfer()");
            EventLogger.WriteToEventLog(this.ServiceName, "Transfer()", EventLogEntryType.Information);

            if (IsFileReady(sourcePath))
            {

               
                string destinationPath = Path.Combine(destination, fileName);
                try
                {
                    File.Copy(sourcePath, destinationPath, true);
                    File.Delete(sourcePath);
                    Log.Information($"Moved: {fileName}");
                    EventLogger.WriteToEventLog(this.ServiceName, $"Moved: {fileName}", EventLogEntryType.Information);

                }
                catch (Exception ex)
                {

                    Log.Error($"Error transferring {fileName}: {ex.Message}");
                    EventLogger.WriteToEventLog(this.ServiceName, $"Error transferring {fileName}: {ex.Message}", EventLogEntryType.Error);

                }
            }
        }

        private bool IsFileReady(string filePath)
        {
            try
            {
                using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None)) { }
                return true;
            }
            catch
            {
               // Log.Information($"File not ready: {filePath}\"");
                return false;
            }
        }
    }


}
