using SomeOneSpyingOnYou.Services;
using System;

namespace SomeOneSpyingOnYou
{
    public class Program
    {
        protected static IEmailService _emailService;

        static void InitializeConsoleConfigurations()
        {
            // Initialize Services
            _emailService = new EmailService();

            // Console Configurations
            Console.ForegroundColor = AppConstants.foregroundColor;
            Console.Title = AppConstants.applicationName;
        }

        static void Main(string[] args)
        {
            InitializeConsoleConfigurations();

            var sender = new EmailCredentials() { Username = "", Password = "" };
            var receivers = new string[] { "atacan.celikkol@hotmail.com" };

            _emailService.SendMailAsync(sender, receivers, "Merhaba canım");
            Console.Read();
        }
    }
}
