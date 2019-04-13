using System;
using System.Threading.Tasks;

namespace SomeOneSpyingOnYou.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(EmailCredentials sender, EmailCredentials receiver, string message);
    }
    public class EmailService : IEmailService
    {
        public async Task<bool> SendEmailAsync(EmailCredentials sender, EmailCredentials receiver, string message)
        {
            Console.WriteLine($"{nameof(SendEmailAsync)}: I'm worked!");
            return true;
        }
    }
}
