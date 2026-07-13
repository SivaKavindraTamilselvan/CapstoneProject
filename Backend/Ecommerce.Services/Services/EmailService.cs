using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Ecommerce.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly string _frontendBaseUrl;
        private readonly ILogger<EmailService> _logger;

        public EmailService(
            IOptions<EmailSettings> emailSettings,
            IConfiguration configuration,
            ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _frontendBaseUrl = configuration["AppSettings:FrontendBaseUrl"] ?? string.Empty;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
                message.To.Add(MailboxAddress.Parse(toEmail));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder { HtmlBody = htmlBody };
                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();

                await client.ConnectAsync(
                    _emailSettings.SmtpHost,
                    _emailSettings.SmtpPort,
                    _emailSettings.UseSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);

                await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation("Email sent successfully to {ToEmail} with subject {Subject}", toEmail, subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {ToEmail}", toEmail);
                throw new EmailSendException($"Failed to send email to {toEmail}");
            }
        }

        public async Task SendSetPasswordEmailAsync(string toEmail, string firstName, string token)
        {
            var setPasswordUrl = $"{_frontendBaseUrl}/set-password?token={Uri.EscapeDataString(token)}";

            var htmlBody = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto;'>
                    <h2 style='color:#1e293b;'>Welcome, {firstName}!</h2>
                    <p>An account has been created for you on the Ecommerce Admin Panel.</p>
                    <p>Please click the button below to set your password and activate your account.</p>
                    <a href='{setPasswordUrl}'
                       style='display:inline-block; background:#1e3a8a; color:#fff; padding:12px 24px;
                              text-decoration:none; border-radius:8px; font-weight:bold; margin-top:12px;'>
                        Set Your Password
                    </a>
                    <p style='margin-top:16px; color:#6b7280; font-size:13px;'>
                        This link will expire in 48 hours. If you did not expect this email, please ignore it.
                    </p>
                </div>";

            await SendEmailAsync(toEmail, "Set Up Your Account Password", htmlBody);
        }

        public async Task SendPasswordResetEmailAsync(string toEmail, string firstName, string token)
        {
            var resetUrl = $"{_frontendBaseUrl}/reset-password?token={Uri.EscapeDataString(token)}";

            var htmlBody = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto;'>
                    <h2 style='color:#1e293b;'>Hello, {firstName}</h2>
                    <p>We received a request to reset your password.</p>
                    <a href='{resetUrl}'
                       style='display:inline-block; background:#1e3a8a; color:#fff; padding:12px 24px;
                              text-decoration:none; border-radius:8px; font-weight:bold; margin-top:12px;'>
                        Reset Password
                    </a>
                    <p style='margin-top:16px; color:#6b7280; font-size:13px;'>
                        This link will expire in 1 hour. If you did not request this, please ignore this email.
                    </p>
                </div>";

            await SendEmailAsync(toEmail, "Reset Your Password", htmlBody);
        }
    }
}