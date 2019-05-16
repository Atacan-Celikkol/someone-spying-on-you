using SomeOneSpyingOnYou.Services;
using System;
using System.Linq;

namespace SomeOneSpyingOnYou
{
    public class Program
    {
        protected static IEmailService _emailService;
        protected static IUserSerivce _userSerivce;

        static void InitializeConsoleConfigurations()
        {
            // Initialize Services
            _emailService = new EmailService();
            _userSerivce = new UserService();

            // Console Configurations
            Console.ForegroundColor = AppConstants.foregroundColor;
            Console.Title = AppConstants.applicationName;
        }

        static void Main(string[] args)
        {
            InitializeConsoleConfigurations();

            var sender = new EmailCredentials()
            {
                Username = ProjectConfigurationManager.SenderAddress,
                Password = ProjectConfigurationManager.SenderPassword
            };

            var receivers = ProjectConfigurationManager.Receivers;

            if (
                args.Any(x => x == "reset")
                || string.IsNullOrEmpty(sender.Username)
                || string.IsNullOrEmpty(sender.Password)
                || string.IsNullOrEmpty(receivers)
               )
            {
                var tuple = _userSerivce.Register();
                sender.Username = tuple.Item1.Username;
                sender.Password = tuple.Item1.Password;
                receivers = tuple.Item2;
            }

            _emailService.SendMailAsync(sender, receivers, $"{Environment.MachineName} is started at {DateTime.Now.ToShortTimeString()}").Wait();
        }
    }
}
