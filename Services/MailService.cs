using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using FYP_Link.Models.Settings;

namespace FYP_Link.Services
{
    public class MailService : IMailService
    {
        private readonly MailSettings _mailSettings;

        public MailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task SendWelcomeEmailAsync(string toEmail, string name)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_mailSettings.FromName, _mailSettings.FromAddress));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = "Welcome to FYP-Link!";

            var builder = new BodyBuilder();
            builder.HtmlBody = $@"
                <h2>Welcome to FYP-Link, {name}!</h2>
                <p>Your supervisor account has been created successfully.</p>
                <p>You can now log in using:</p>
                <ul>
                    <li><strong>Email:</strong> {toEmail}</li>
                    <li><strong>Password:</strong> Password123!</li>
                </ul>
                <p><strong>Important:</strong> For security reasons, please change your password immediately after your first login.</p>
                <p>Best regards,<br>FYP-Link Team</p>";

            email.Body = builder.ToMessageBody();

            await SendEmailAsync(email);
        }

        public async Task SendVerificationEmailAsync(string toEmail, string name, string verificationLink)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_mailSettings.FromName, _mailSettings.FromAddress));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = "Verify your FYP-Link account";

            var builder = new BodyBuilder();
            builder.HtmlBody = $@"
                <h2>Welcome to FYP-Link, {name}!</h2>
                <p>Please verify your email address by clicking the link below:</p>
                <p><a href='{verificationLink}'>Verify Email</a></p>
                <p>If you did not create this account, please ignore this email.</p>
                <p>Best regards,<br>FYP-Link Team</p>";

            email.Body = builder.ToMessageBody();

            await SendEmailAsync(email);
        }

        private async Task SendEmailAsync(MimeMessage email)
        {
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_mailSettings.UserName, _mailSettings.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
} 