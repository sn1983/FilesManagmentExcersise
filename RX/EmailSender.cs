using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RX
{
    /// <summary>
    /// Provides functionality to send emails using the specified mail settings.
    /// </summary>
    public static class EmailSender
    {
        /// <summary>
        /// Sends an email using the provided <see cref="MailSettings"/>, subject, and body.
        /// </summary>
        /// <param name="settings">The mail settings containing SMTP server, credentials, and sender/recipient information.</param>
        /// <param name="subject">The subject of the email.</param>
        /// <param name="body">The body content of the email.</param>
        public static void SendEmail(MailSettings settings, string subject, string body)
        {
            using (var client = new SmtpClient(settings.SmtpServer, settings.Port))
            {
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(settings.Username, settings.Password);
                client.EnableSsl = settings.UseSsl;

                using (var message = new MailMessage())
                {
                    message.From = new MailAddress(settings.SenderEmail, settings.SenderName);
                    message.To.Add(settings.RecpientEmail);
                    message.Subject = subject;
                    message.Body = body;
                    message.IsBodyHtml = true;

                    client.Send(message);
                }
            }
        }
    }
}
