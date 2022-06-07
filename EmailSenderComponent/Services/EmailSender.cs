using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Threading.Tasks;


namespace EmailSenderComponent.Services
{
    public class EmailSender : IEmailSender
    {

        private readonly string _host;
        private readonly int _port;
        private readonly bool _ssl;
        private readonly string _username;
        private readonly string _password;
        public EmailSender(string host, int port, bool ssl, string username, string password)
        {
            _host = host;
            _port = port;
            _ssl = ssl;
            _username = username;
            _password = password;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage, List<string> attachments)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("IGC", "kamran.alibayli@igc.com.tr"));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = subject;
            message.Body = new TextPart("html")
            {
                Text = htmlMessage
            };

            var bodyBuilder = new BodyBuilder();

            if (attachments != null)
            {

                foreach (var attachment in attachments)
                {
                    bodyBuilder.Attachments.Add(attachment);

                }
            }

            message.Body = bodyBuilder.ToMessageBody();

            using (var smtp = new SmtpClient())
            {
                try
                {
                    smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    smtp.CheckCertificateRevocation = false;
                    smtp.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13;
                    smtp.Connect(_host, _port, _ssl);
                    smtp.Authenticate(_username, _password);
                    await smtp.SendAsync(message);
                    smtp.Disconnect(true);
                }
                catch (Exception e)
                {

                    throw;
                }
            }

        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            throw new NotImplementedException();
        }
    }



}
