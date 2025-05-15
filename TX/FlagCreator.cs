using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

namespace TX
{
    /// <summary>
    /// Responsible for creating flag files in a specified folder at regular intervals.
    /// </summary>
    public class FlagCreator
    {
        /// <summary>
        /// Gets or sets the folder where flag files will be created.
        /// </summary>
        public string Folder { get; set; }

        /// <summary>
        /// Gets or sets the name of the service using this flag creator.
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FlagCreator"/> class.
        /// </summary>
        /// <param name="Folder">The folder where flag files will be created.</param>
        /// <param name="ServiceName">The name of the service.</param>
        public FlagCreator(string Folder, string ServiceName)
        {
            this.Folder = Folder;
            this.ServiceName = ServiceName;
        }

        /// <summary>
        /// Starts a timer that periodically creates a flag file with the current timestamp.
        /// </summary>
        public void CreateFlagFile()
        {
            Log.Information("CreateFlagFile()");
            EventLogger.WriteToEventLog(this.ServiceName, "CreateFlagFile()", EventLogEntryType.Information);

            Random random = new Random();
            int number = random.Next(5, 11); // 11 is exclusive, so it returns 5–10
            System.Timers.Timer timer = new System.Timers.Timer(number * 1000);

            Task.Run(() =>
            {
                timer.AutoReset = true;
                timer.Enabled = true;

                timer.Elapsed += (sender, e) =>
                {
                    CreateFileWithCurrentTimeStamp();
                };

                Console.WriteLine("Timer started in Task. Press Enter to stop.");
            });
        }

        /// <summary>
        /// Creates a flag file in the specified folder with the current timestamp and service name.
        /// </summary>
        /// <returns>True if the file was created successfully; otherwise, false.</returns>
        bool CreateFileWithCurrentTimeStamp()
        {
            try
            {
                Log.Information("CreateFileWithCurrentTimeStamp()");
                EventLogger.WriteToEventLog(this.ServiceName, "CreateFileWithCurrentTimeStamp()", EventLogEntryType.Information);

                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string fileName = $"flag_{timestamp}.txt";
                string filePath = Path.Combine(Folder, fileName);

                File.WriteAllText(filePath, $"Time Stamp: {DateTime.Now.ToString("yyyyMMdd_HHmmss")}\nService Name: {ServiceName}");

                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"CreateFileWithCurrentTimeStamp() Exception: {ex.Message}\n Stack Trace: {ex.StackTrace}");
                EventLogger.WriteToEventLog(this.ServiceName, $"CreateFileWithCurrentTimeStamp() Exception: {ex.Message}\n Stack Trace: {ex.StackTrace}", EventLogEntryType.Error);

                return false;
            }
        }
    }
}
