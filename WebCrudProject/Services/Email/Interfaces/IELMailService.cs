using WebCrudProject.Auth.Models;

namespace WebCrudProject.Services.Email.Interfaces
{
    public interface IELMailService
    {
        Task SendAccountRegistrationAsyn(ELUser user);

        Task SendChangePasswordAsync(string email);
    }
}
