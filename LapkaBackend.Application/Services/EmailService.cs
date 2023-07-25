using LapkaBackend.Application.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using LapkaBackend.Application.Helper;

namespace LapkaBackend.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly IEmailWrapper _emailWrapper;
        public EmailService(IOptions<EmailSettings> options, IEmailWrapper emailWrapper)
        {
            _emailSettings = options.Value;
            _emailWrapper = emailWrapper;
        }

        public async Task SendEmail(MailRequest mailRequest)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_emailSettings.Email);
            email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
            email.Subject = mailRequest.Subject;
            var builder = _emailWrapper.GetBuilder(mailRequest.Template);
            builder.HtmlBody = builder.HtmlBody.Replace("__link", mailRequest.RedirectUrl);
            
            email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_emailSettings.Host, _emailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_emailSettings.Email, _emailSettings.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
