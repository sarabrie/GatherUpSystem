using GatherUp.Core.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;

namespace GatherUp.BL.Services
{
    public class SmtpMailService : IMailService
    {
        private readonly string _host;
        private readonly int _port;
        private readonly string _username;
        private readonly string _password;
        private readonly string _fromEmail;

        public SmtpMailService(string host, int port, string username, string password, string fromEmail)
        {
            _host = host;
            _port = port;
            _username = username;
            _password = password;
            _fromEmail = fromEmail;
        }

        public void Send(string to, string subject, string body)
        {
            MimeMessage message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(_fromEmail));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;
            message.Body = new TextPart("plain") { Text = body };

            using SmtpClient client = new SmtpClient();
            client.Connect(_host, _port, MailKit.Security.SecureSocketOptions.StartTls);
            client.Authenticate(_username, _password);
            client.Send(message);
            client.Disconnect(true);
        }
    }
}
