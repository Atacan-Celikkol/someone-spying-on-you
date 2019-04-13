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
            _emailService.SendEmailAsync(null,null,null);
            Console.Read();
        }
    }
}
