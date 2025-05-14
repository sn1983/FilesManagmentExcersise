namespace  ClassHelpers
{
    public class TransferSettings
    {
        public string SourceFolder { get; set; }
        public string DestinationFolder { get; set; }
        public bool SysLog { get; set; }

        public TransferSettings() { }

        public TransferSettings(string sourceFolder, string destinationFolder, bool sysLog)
        {
            SourceFolder = sourceFolder;
            DestinationFolder = destinationFolder;
            SysLog = sysLog;
        }
    }
}
