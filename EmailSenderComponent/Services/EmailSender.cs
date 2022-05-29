using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;


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

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("IGC", "kamran.alibayli@igc.com.tr"));
            message.To.Add(new MailboxAddress("Frenk",email));

            message.Subject = subject;
            message.Body = new TextPart("html")
            {
                Text = htmlMessage
            };

            using (var smtp = new SmtpClient())
            {
                try
                {
                smtp.Connect(_host, _port, _ssl);
                smtp.Authenticate(_username, _password);
                await smtp.SendAsync(message);
                smtp.Disconnect(true);
                }
                catch (Exception ex)
                {

                    throw;
                }
            }

        }
    }


  
}
