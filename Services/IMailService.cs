namespace FYP_Link.Services
{
    public interface IMailService
    {
        Task SendWelcomeEmailAsync(string toEmail, string name);
        Task SendVerificationEmailAsync(string toEmail, string name, string verificationLink);
        Task SendPasswordResetLinkAsync(string toEmail, string resetLink);
    }
} 