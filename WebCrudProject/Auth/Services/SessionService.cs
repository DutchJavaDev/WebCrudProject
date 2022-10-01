using WebCrudProject.Auth.Models;
using WebCrudProject.Auth.Services.Interfaces;
using WebCrudProject.Services.ORM.Interfaces;

namespace WebCrudProject.Auth.Services
{
    public class SessionService : ISessionService
    {
        private readonly IObjectContext _context;
        public SessionService(IObjectContext objectContext)
        {
            _context = objectContext;
        }
        public async Task<string> CreateSessionAsync(ELUser requester)
        {
            var created = DateTime.Now;
            var expired = created.AddSeconds(30);
            var id = Guid.NewGuid().ToString().Replace("-","");

            var session = new ELUJwtSession 
            {
                UserId = requester.Id,
                DateCreated = created,
                ExpireDate = expired,
                SessionId = id,
                CompressedRoles = requester.CompressedRoles,
                Email = requester.Email,
                LastUpdated = created,
            };

            await _context.InsertAsync(session);

            return id;
        }

        public async Task DeleteSessionAsync(int sessionId)
        {
            var session = _context.GetByIdAsync<ELUJwtSession>(sessionId);

            await _context.DeleteAsync(session)
                .ConfigureAwait(false);
        }

        public async Task<(bool,ELUJwtSession)> ResolveSessionAsync(string sessionId)
        {
            var sessions = (await _context.GetListAsync<ELUJwtSession>())
                .Where(i => i.SessionId == sessionId);

            if (sessions.Any())
            {
                var session = sessions.First();

                if (session.IsExpired())
                {
                    await _context.DeleteAsync(session);
                    return (false, new ELUJwtSession { ExpireDate = DateTime.Now.AddMinutes(-1) });
                }

                return (!session.IsExpired(), session);
            }

            return (false, new ELUJwtSession { ExpireDate = DateTime.Now.AddMinutes(-1) });
        }

        
    }
}
