using System.Net.Mail;
using System.Net;

namespace Parcel.Processing.Utilities
{
    public class SMTPEmailConfiguration
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string HostAddress { get; set; }
        public int HostPort { get; set; }
    }

    /// <summary>
    /// Utility functions for sending an email
    /// </summary>
    public static class EmailHelper
    {
        public static void SendEmail(string sender, string[] receipients, string title, string body, SMTPEmailConfiguration configurations)
        {
            SmtpClient client = new(configurations.HostAddress, configurations.HostPort)
            {
                Credentials = new NetworkCredential(configurations.Username, configurations.Password),
                EnableSsl = true
            };
            client.Send(sender, string.Join(";", receipients), title, body);
        }
        public static void SendHTMLEmail(string sender, string[] receipients, string title, string body, SMTPEmailConfiguration configurations)
        {
            SmtpClient client = new(configurations.HostAddress, configurations.HostPort)
            {
                Credentials = new NetworkCredential(configurations.Username, configurations.Password),
                EnableSsl = true
            };
            MailMessage message = new(sender, string.Join(";", receipients), title, body)
            {
                IsBodyHtml = true
            };
            client.Send(message);
        }
    }
}
