using WebCrudProject.Auth.Models;

namespace WebCrudProject.Auth.Services.Interfaces
{
    public interface ISessionService
    {
        Task<string> CreateSessionAsync(ELUser requester);

        Task<(bool,ELUJwtSession)> ResolveSessionAsync(string sessionId);

        Task DeleteSessionAsync(int sessionId);
    }
}
