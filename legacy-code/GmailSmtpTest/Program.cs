using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace GmailSmtpTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });
            var logger = loggerFactory.CreateLogger<GmailSmtpEmailSender>();

            var emailSender = new GmailSmtpEmailSender(configuration, logger);

            await emailSender.SendEmailAsync(
                "goldenduckling13@gmail.com",
                "🔥 Gmail SMTP Test",
                "<h3>Chúc bạn một ngày tốt lành! Đây là email test từ Gmail SMTP</h3>"
            );
        }
    }
}
