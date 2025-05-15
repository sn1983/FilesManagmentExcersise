namespace  ClassHelpers
{
    /// <summary>
    /// Represents configuration settings for file transfer operations,
    /// including source and destination folders and system logging option.
    /// </summary>
    public class TransferSettings
    {
        /// <summary>
        /// Gets or sets the source folder path for file transfers.
        /// </summary>
        public string SourceFolder { get; set; }

        /// <summary>
        /// Gets or sets the destination folder path for file transfers.
        /// </summary>
        public string DestinationFolder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether system logging is enabled.
        /// </summary>
        public bool SysLog { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransferSettings"/> class.
        /// </summary>
        public TransferSettings() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransferSettings"/> class with specified values.
        /// </summary>
        /// <param name="sourceFolder">The source folder path.</param>
        /// <param name="destinationFolder">The destination folder path.</param>
        /// <param name="sysLog">Indicates whether system logging is enabled.</param>
        public TransferSettings(string sourceFolder, string destinationFolder, bool sysLog)
        {
            SourceFolder = sourceFolder;
            DestinationFolder = destinationFolder;
            SysLog = sysLog;
        }
    }
}
