using SomeOneSpyingOnYou.Services;
using System;
using System.Linq;

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

            if (
                args.Any(x => x == "reset")
                || string.IsNullOrEmpty(ProjectConfigurationManager.SenderAddress)
                || string.IsNullOrEmpty(ProjectConfigurationManager.SenderPassword)
                || string.IsNullOrEmpty(ProjectConfigurationManager.Receivers)
               )
            {
                RegisterUserInformation();
            }

            var sender = new EmailCredentials()
            {
                Username = ProjectConfigurationManager.SenderAddress,
                Password = ProjectConfigurationManager.SenderPassword
            };
            _emailService.SendMailAsync(sender, ProjectConfigurationManager.Receivers, "Merhaba canım");
        }

        static void RegisterUserInformation()
        {
            var tuple = GetInputs();

            ProjectConfigurationManager.AddOrReplace("Sender.Address", tuple.Item1);
            ProjectConfigurationManager.AddOrReplace("Sender.Password", tuple.Item2);
            ProjectConfigurationManager.AddOrReplace("Receivers", tuple.Item3);
        }

        /// <summary>
        /// Item1=SenderAddress,
        /// Item2=SenderPassword,
        /// Item3=Receivers,
        /// Item4=PasswordHash
        /// </summary>
        /// <returns></returns>
        static Tuple<string, string, string> GetInputs()
        {
            Console.WriteLine("Welcome, please give me some information");

            Console.Write("Sender email address: ");
            var senderAddress = Console.ReadLine();
            Console.WriteLine("");

            Console.Write("Sender email password: ");
            var senderPassword = Console.ReadLine();
            Console.Clear();

            Console.WriteLine("Please enter receiver address or addresses.");
            Console.WriteLine("You can separate the addresses with ',' e.g. atacan@github.com,ata@github.com");
            var receivers = Console.ReadLine();            

            using (CryptographyService cryptographyService = new CryptographyService())
            {
                senderPassword = cryptographyService.Encrypt(senderPassword);
            }

            return new Tuple<string, string, string>(senderAddress, senderPassword, receivers);
        }
    }
}
