namespace ClassHelpers
{
    public class RxSettings
    {
        public string DestinationFolder { get; set; }
        public bool SysLog { get; set; }
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public bool UseSsl { get; set; }
        public string SenderEmail { get; set; }
        public string RecpientEmail { get; set; }   
        public string SenderName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public string logFolder { get; set; }   
        public RxSettings() { }

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
