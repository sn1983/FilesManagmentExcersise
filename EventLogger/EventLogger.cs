using System;
using System.Diagnostics;

public static class EventLogger
{
    public static bool WriteToEventLog(
        string source,
        string message,
        EventLogEntryType type = EventLogEntryType.Information,
        string logName = "Application")
    {
        try
        {
            // Ensure the event source exists
            if (!EventLog.SourceExists(source))
            {
                try
                {
                    EventLog.CreateEventSource(source, logName);
                    // Source creation requires a restart to take effect
                    return false;
                }
                catch (Exception ex)
                {
                    // Another process may have created the source
                    if (!EventLog.SourceExists(source))
                        throw new InvalidOperationException($"Failed to create event source '{source}'.", ex);
                }
            }

            using (var eventLog = new EventLog(logName) { Source = source })
            {
                eventLog.WriteEntry(message, type);
            }

            return true;
        }
        catch (Exception)
        {
            // Optionally, log to a fallback mechanism here
            return false;
        }
    }
}
