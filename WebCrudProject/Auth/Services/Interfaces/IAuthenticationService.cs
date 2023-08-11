using WebCrudProject.Auth.Models;

namespace WebCrudProject.Auth.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<bool> RegisterAsync(AuthenticationModel model);

        Task<ELUser> LoginAsync(AuthenticationModel model);
    }
}
