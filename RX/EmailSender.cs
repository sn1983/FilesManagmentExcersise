using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RX
{
    public static class EmailSender
    {
        public static void SendEmail(MailSettings settings, string to, string subject, string body)
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
                    message.To.Add(to);
                    message.Subject = subject;
                    message.Body = body;
                    message.IsBodyHtml = true;

                    client.Send(message);
                }
            }
        }

    }
}
