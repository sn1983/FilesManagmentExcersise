using System;
using System.Diagnostics;

public static class EventLogger
{
    public static void WriteToEventLog(string source, string message, EventLogEntryType type = EventLogEntryType.Information, string logName = "Application")
    {
        try
        {
            // Create source if it doesn't exist
            if (!EventLog.SourceExists(source))
            {
                EventLog.CreateEventSource(source, logName);
                Console.WriteLine($"Event source '{source}' created. Please rerun the application to write the log entry.");
                return;
            }

            // Write the log entry
            EventLog.WriteEntry(source, message, type);
            Console.WriteLine("Event written successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to write to event log: {ex.Message}");
        }
    }
}
