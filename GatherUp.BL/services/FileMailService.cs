using System;
using System.IO;
using GatherUp.Core.Interfaces;

namespace GatherUp.Infrastructure.Services
{
    public class FileMailService : IMailService
    {
        private readonly string _filePath = "mail_log.txt";

        public void Send(string to, string subject, string body)
        {
            string emailContent = $"[{DateTime.Now}] To: {to} | Subject: {subject}\nBody: {body}\n\n";
            File.AppendAllText(_filePath, emailContent);
        }
    }
}