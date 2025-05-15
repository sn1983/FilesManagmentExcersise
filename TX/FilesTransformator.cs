using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace TX
{
    using System.Diagnostics;

    /// <summary>
    /// Implements file transfer operations using the local file system.
    /// Monitors a source folder and moves new or changed files to a destination folder.
    /// </summary>
    public class LocalFileTransfer : IFileTransfer
    {
        private readonly string source;
        private readonly string destination;
        private FileSystemWatcher watcher;

        /// <summary>
        /// Gets the name of the service using this file transfer.
        /// </summary>
        private string ServiceName { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalFileTransfer"/> class.
        /// </summary>
        /// <param name="sourceFolder">The source folder to watch for files.</param>
        /// <param name="destinationFolder">The destination folder to move files to.</param>
        /// <param name="ServiceName">The name of the service using this transfer.</param>
        public LocalFileTransfer(string sourceFolder, string destinationFolder, string ServiceName)
        {
            source = sourceFolder;
            destination = destinationFolder;
            this.ServiceName = ServiceName ?? string.Empty;
        }

        /// <summary>
        /// Starts watching the source folder for new or changed files and initiates transfer when detected.
        /// </summary>
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

        /// <summary>
        /// Transfers a file from the source folder to the destination folder if the file is ready.
        /// </summary>
        /// <param name="sourcePath">The full path of the source file.</param>
        /// <param name="fileName">The name of the file to transfer.</param>
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

        /// <summary>
        /// Checks if a file is ready to be accessed (i.e., not locked by another process).
        /// </summary>
        /// <param name="filePath">The full path of the file to check.</param>
        /// <returns>True if the file is ready; otherwise, false.</returns>
        private bool IsFileReady(string filePath)
        {
            try
            {
                using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None)) { }
                return true;
            }
            catch
            {
                // File is not ready
                return false;
            }
        }
    }
}
