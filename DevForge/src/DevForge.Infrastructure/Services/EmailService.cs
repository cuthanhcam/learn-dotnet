using DevForge.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

namespace DevForge.Infrastructure.Services
{
    /// <summary>
    /// Email-based implementation of notification service using SMTP
    /// </summary>
    public class EmailNotificationService : INotificationService
    {
        private readonly ILogger<EmailNotificationService> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly string _fromEmail;
        private readonly string _fromName;
        private readonly bool _enableSsl;

        public EmailNotificationService(ILogger<EmailNotificationService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;

            // Load SMTP configuration from appsettings
            _smtpHost = _configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
            _smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
            _smtpUsername = _configuration["Email:SmtpUsername"] ?? "";
            _smtpPassword = _configuration["Email:SmtpPassword"] ?? "";
            _fromEmail = _configuration["Email:FromEmail"] ?? "noreply@devforge.com";
            _fromName = _configuration["Email:FromName"] ?? "DevForge";
            _enableSsl = bool.Parse(_configuration["Email:EnableSsl"] ?? "true");
        }

        public async Task SendEmailConfirmationAsync(string email, string username, string confirmationLink, CancellationToken cancellationToken = default)
        {
            try
            {
                var subject = "Confirm Your Email - DevForge";
                var body = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif;'>
                        <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                            <h2 style='color: #333;'>Welcome to DevForge, {username}!</h2>
                            <p>Thank you for registering. Please confirm your email address by clicking the button below:</p>
                            <div style='text-align: center; margin: 30px 0;'>
                                <a href='{confirmationLink}' 
                                   style='background-color: #4CAF50; color: white; padding: 14px 28px; 
                                          text-decoration: none; border-radius: 4px; display: inline-block;'>
                                    Confirm Email
                                </a>
                            </div>
                            <p>Or copy and paste this link into your browser:</p>
                            <p style='color: #666; word-break: break-all;'>{confirmationLink}</p>
                            <p style='margin-top: 30px; color: #999; font-size: 12px;'>
                                If you didn't create an account, please ignore this email.
                            </p>
                        </div>
                    </body>
                    </html>";

                await SendEmailAsync(email, subject, body, cancellationToken);
                _logger.LogInformation("Email confirmation sent to {Email} for user {Username}", email, username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email confirmation to {Email}", email);
                throw;
            }
        }

        public async Task SendPasswordResetAsync(string email, string username, string resetLink, CancellationToken cancellationToken = default)
        {
            try
            {
                // Prepend frontend URL if resetLink is relative
                var fullResetLink = resetLink.StartsWith("http") 
                    ? resetLink 
                    : $"{(_configuration["Application:FrontendUrl"] ?? "http://localhost:4200")}{resetLink}";

                var subject = "Password Reset Request - DevForge";
                var body = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif;'>
                        <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                            <h2 style='color: #333;'>Password Reset Request</h2>
                            <p>Hello {username},</p>
                            <p>We received a request to reset your password. Click the button below to reset it:</p>
                            <div style='text-align: center; margin: 30px 0;'>
                                <a href='{fullResetLink}' 
                                   style='background-color: #2196F3; color: white; padding: 14px 28px; 
                                          text-decoration: none; border-radius: 4px; display: inline-block;'>
                                    Reset Password
                                </a>
                            </div>
                            <p>Or copy and paste this link into your browser:</p>
                            <p style='color: #666; word-break: break-all;'>{fullResetLink}</p>
                            <p style='margin-top: 30px; color: #f44336;'>
                                <strong>This link will expire in 1 hour.</strong>
                            </p>
                            <p style='color: #999; font-size: 12px;'>
                                If you didn't request a password reset, please ignore this email or contact support.
                            </p>
                        </div>
                    </body>
                    </html>";

                await SendEmailAsync(email, subject, body, cancellationToken);
                _logger.LogInformation("Password reset email sent to {Email} for user {Username}", email, username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send password reset email to {Email}", email);
                throw;
            }
        }

        public async Task SendWelcomeEmailAsync(string email, string username, CancellationToken cancellationToken = default)
        {
            try
            {
                var subject = "Welcome to DevForge!";
                var body = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif;'>
                        <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                            <h2 style='color: #333;'>Welcome to DevForge, {username}! 🎉</h2>
                            <p>Your email has been confirmed successfully.</p>
                            <p>You can now enjoy all the features of DevForge:</p>
                            <ul style='color: #555;'>
                                <li>Secure authentication with JWT tokens</li>
                                <li>Two-factor authentication for enhanced security</li>
                                <li>Role-based access control</li>
                                <li>And much more!</li>
                            </ul>
                            <p style='margin-top: 30px;'>
                                If you have any questions, feel free to reach out to our support team.
                            </p>
                            <p>Best regards,<br/>The DevForge Team</p>
                        </div>
                    </body>
                    </html>";

                await SendEmailAsync(email, subject, body, cancellationToken);
                _logger.LogInformation("Welcome email sent to {Email} for user {Username}", email, username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send welcome email to {Email}", email);
                throw;
            }
        }

        /// <summary>
        /// Core method to send emails via SMTP
        /// </summary>
        private async Task SendEmailAsync(string toEmail, string subject, string htmlBody, CancellationToken cancellationToken = default)
        {
            // If SMTP is not configured, just log and return (development mode)
            if (string.IsNullOrEmpty(_smtpUsername) || string.IsNullOrEmpty(_smtpPassword))
            {
                _logger.LogWarning("SMTP not configured. Email would be sent to {Email} with subject '{Subject}'", toEmail, subject);
                return;
            }

            using var message = new MailMessage
            {
                From = new MailAddress(_fromEmail, _fromName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };

            message.To.Add(toEmail);

            using var smtpClient = new SmtpClient(_smtpHost, _smtpPort)
            {
                Credentials = new NetworkCredential(_smtpUsername, _smtpPassword),
                EnableSsl = _enableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            await smtpClient.SendMailAsync(message, cancellationToken);
        }
    }
}
