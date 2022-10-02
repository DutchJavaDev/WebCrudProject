using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using WebCrudProject.Auth.Models;
using WebCrudProject.Services.Email.Interfaces;

namespace WebCrudProject.Services.Email
{
    public sealed class ELMailService : IELMailService
    {
        private readonly string _host;
        private readonly int _port;
        private readonly string _user;
        private readonly string _password;

        public ELMailService(IConfiguration configuration)
        {
            _host = configuration["EmailHost"];
            _port = int.Parse(configuration["EmailSmtpPort"]);
            _user = configuration["EmailUser"];
            _password = configuration["EmailPassword"];
        }

        public async Task SendAccountRegistrationAsyn(ELUser user)
        {
            using (var client = await CreateClient())
            {
                var accountRegistration = new MimeMessage();

                accountRegistration.From.Add(MailboxAddress.Parse(_user));
                accountRegistration.To.Add(MailboxAddress.Parse(user.Email));

                accountRegistration.Subject = "Account created";
                accountRegistration.Body = new TextPart(TextFormat.Html) 
                {
                    Text = $"{user.Email} has been created! " +
                    $"{user.DateCreated} {user.CompressedRoles}"
                };

                await client.SendAsync(accountRegistration);
                await client.DisconnectAsync(true);
            }
        }

        public async Task SendChangePasswordAsync(string email)
        {
            // ?
        }

        private async Task<SmtpClient> CreateClient()
        {
            var client = new SmtpClient();
            await client.ConnectAsync(_host,_port,MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_user, _password);
            return client;
        }
    }
}
