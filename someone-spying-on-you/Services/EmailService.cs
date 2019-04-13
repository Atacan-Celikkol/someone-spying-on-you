using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SomeOneSpyingOnYou.Services
{
    public interface IEmailService
    {
        Task SendMailAsync(EmailCredentials sender, string[] receiverAddresses, string text);
    }
    public class EmailService : IEmailService
    {
        public async Task SendMailAsync(EmailCredentials sender, string[] receiverAddresses, string text)
        {
            if (receiverAddresses.Length < 1)
            {
                throw new ArgumentException(nameof(receiverAddresses));
            }

            var message = await InitializeAddresses(sender.Username, receiverAddresses);
            message.Subject = AppConstants.applicationName;
            message.Body = text;

            var client = await InitializeSmtpClient(sender);

            await client.SendMailAsync(message);
            Console.WriteLine("Email sent");
        }

        #region Private Methods
        private async Task<MailMessage> InitializeAddresses(string senderAddress, string[] receiverAddresses)
        {
            var message = new MailMessage();
            foreach (var address in receiverAddresses)
            {
                var receiverMailAddress = new MailAddress(address);
                message.To.Add(receiverMailAddress);
            }

            var senderMailAddress = new MailAddress(senderAddress);
            message.Sender = senderMailAddress;
            message.From = senderMailAddress;

            return message;
        }

        private async Task<SmtpClient> InitializeSmtpClient(EmailCredentials sender)
        {
            var client = new SmtpClient();
            client.Credentials = new NetworkCredential()
            {
                UserName = sender.Username,
                Password = sender.Password
            };

            client.Port = ProjectConfigurationManager.DefaultSmtpPort;
            client.Host = ProjectConfigurationManager.DefaultSmtpHost;
            client.EnableSsl = ProjectConfigurationManager.DefaultSmtpSSL;

            return client;
        }
        #endregion
    }
}
