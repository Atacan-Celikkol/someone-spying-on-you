using System;

namespace SomeOneSpyingOnYou.Services
{
    public interface IUserSerivce
    {
        EmailCredentials GetSenderInformation();
        string GetReceivers();
        /// <summary>
        /// Item1=SenderCredentials
        /// Item2=Receivers
        /// </summary>
        Tuple<EmailCredentials, string> Register();
    }
    public class UserService : IUserSerivce
    {

        public Tuple<EmailCredentials, string> Register()
        {
            var sender = UpdateSenderInformation();
            var receivers = UpdateReceivers();

            return new Tuple<EmailCredentials, string>(sender, receivers);
        }

        public EmailCredentials GetSenderInformation()
        {
            var username = ProjectConfigurationManager.SenderAddress;
            var password = ProjectConfigurationManager.SenderPassword;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return UpdateSenderInformation();
            }

            return new EmailCredentials()
            {
                Username = username,
                Password = password
            };
        }       

        public string GetReceivers()
        {
            var receivers = ProjectConfigurationManager.Receivers;
            return string.IsNullOrEmpty(receivers) ? UpdateReceivers() : receivers;
        }

        #region Private Methods
        private EmailCredentials UpdateSenderInformation()
        {
            Console.Write("Sender email address: ");
            var senderAddress = Console.ReadLine();
            Console.WriteLine("");

            Console.Write("Sender email password: ");
            var senderPassword = Console.ReadLine();
            Console.Clear();

            using (CryptographyService cryptographyService = new CryptographyService())
            {
                senderPassword = cryptographyService.Encrypt(senderPassword);
            }

            ProjectConfigurationManager.AddOrReplace("Sender.Address", senderAddress);
            ProjectConfigurationManager.AddOrReplace("Sender.Password", senderPassword);

            return new EmailCredentials() { Username = senderAddress, Password = senderPassword };
        }

        private string UpdateReceivers()
        {
            Console.WriteLine("Please enter receiver address or addresses.");
            Console.WriteLine("You can separate the addresses with ',' e.g. atacan@github.com,ata@github.com");
            var receivers = Console.ReadLine();
            Console.Clear();
            ProjectConfigurationManager.AddOrReplace("Receivers", receivers);
            return receivers;
        }
        #endregion
    }
}
