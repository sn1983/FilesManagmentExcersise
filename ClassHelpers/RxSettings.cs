namespace ClassHelpers
{
    /// <summary>
    /// Represents configuration settings for the RX service, including mail and folder options.
    /// </summary>
    public class RxSettings
    {
        /// <summary>
        /// Gets or sets the destination folder for received files.
        /// </summary>
        public string DestinationFolder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether system logging is enabled.
        /// </summary>
        public bool SysLog { get; set; }

        /// <summary>
        /// Gets or sets the SMTP server address for sending emails.
        /// </summary>
        public string SmtpServer { get; set; }

        /// <summary>
        /// Gets or sets the port number for the SMTP server.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether SSL should be used for SMTP.
        /// </summary>
        public bool UseSsl { get; set; }

        /// <summary>
        /// Gets or sets the sender's email address.
        /// </summary>
        public string SenderEmail { get; set; }

        /// <summary>
        /// Gets or sets the recipient's email address.
        /// </summary>
        public string RecpientEmail { get; set; }

        /// <summary>
        /// Gets or sets the sender's display name.
        /// </summary>
        public string SenderName { get; set; }

        /// <summary>
        /// Gets or sets the username for SMTP authentication.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password for SMTP authentication.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the folder path for log files.
        /// </summary>
        public string logFolder { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RxSettings"/> class.
        /// </summary>
        public RxSettings() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RxSettings"/> class with specified values.
        /// </summary>
        /// <param name="destinationFolder">The destination folder for received files.</param>
        /// <param name="sysLog">Indicates whether system logging is enabled.</param>
        /// <param name="smtpServer">The SMTP server address.</param>
        /// <param name="port">The SMTP server port.</param>
        /// <param name="useSsl">Indicates whether SSL is used for SMTP.</param>
        /// <param name="senderEmail">The sender's email address.</param>
        /// <param name="senderName">The sender's display name.</param>
        /// <param name="username">The username for SMTP authentication.</param>
        /// <param name="password">The password for SMTP authentication.</param>
        /// <param name="recpientEmail">The recipient's email address.</param>
        public RxSettings(
            string destinationFolder,
            bool sysLog,
            string smtpServer,
            int port,
            bool useSsl,
            string senderEmail,
            string senderName,
            string username,
            string password,
            string recpientEmail)
        {
            DestinationFolder = destinationFolder;
            SysLog = sysLog;
            SmtpServer = smtpServer;
            Port = port;
            UseSsl = useSsl;
            SenderEmail = senderEmail;
            SenderName = senderName;
            Username = username;
            Password = password;
            RecpientEmail = recpientEmail;
        }
    }
}
