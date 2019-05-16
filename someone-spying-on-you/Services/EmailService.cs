using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SomeOneSpyingOnYou.Services
{
    public interface IEmailService
    {
        Task SendMailAsync(EmailCredentials sender, string receiverAddresses, string text);
    }
    public class EmailService : IEmailService
    {
        public async Task SendMailAsync(EmailCredentials sender, string receiverAddresses, string text)
        {
            if (receiverAddresses.Length < 1)
            {
                throw new ArgumentException(nameof(receiverAddresses));
            }

            var message = InitializeAddresses(sender.Username, receiverAddresses);
            message.Subject = AppConstants.applicationName;
            message.Body = text;

            var client = InitializeSmtpClient(sender);
            try
            {
                await client.SendMailAsync(message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException);
                Console.ReadLine();
                return;
            }

            Console.WriteLine("Email sent");
        }

        #region Private Methods
        private MailMessage InitializeAddresses(string senderAddress, string receiverAddresses)
        {
            var message = new MailMessage();
            var splitChars = new char[] { ',', ';' };
            var receivers = receiverAddresses.Split(splitChars);
            foreach (var address in receivers)
            {
                var receiverMailAddress = new MailAddress(address);
                message.To.Add(receiverMailAddress);
            }

            var senderMailAddress = new MailAddress(senderAddress);
            message.Sender = senderMailAddress;
            message.From = senderMailAddress;

            return message;
        }

        private SmtpClient InitializeSmtpClient(EmailCredentials sender)
        {
            using (CryptographyService cryptographyService = new CryptographyService())
            {
                sender.Password = cryptographyService.Decrypt(sender.Password);
            }

            var client = new SmtpClient();
            client.Credentials = new NetworkCredential()
            {
                UserName = sender.Username,
                Password = sender.Password,
            };

            client.Port = ProjectConfigurationManager.DefaultSmtpPort;
            client.Host = ProjectConfigurationManager.DefaultSmtpHost;
            client.EnableSsl = ProjectConfigurationManager.DefaultSmtpSSL;

            return client;
        }
        #endregion
    }
}
