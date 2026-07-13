namespace Ecommerce.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string htmlBody);
        Task SendSetPasswordEmailAsync(string toEmail, string firstName, string token);
        Task SendPasswordResetEmailAsync(string toEmail, string firstName, string token);
    }
}