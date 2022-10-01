using WebCrudProject.Auth.Models;
using WebCrudProject.Auth.Services.Interfaces;
using WebCrudProject.Services.ORM.Interfaces;
using BCryptNet = BCrypt.Net.BCrypt;

namespace WebCrudProject.Auth.Services
{
    public sealed class AuthenticationService : IAuthenticationService
    {
        private readonly IObjectContext _context; 
        public AuthenticationService(IObjectContext context)
        {
            _context = context;
        }

        public async Task<ELUser> LoginAsync(AuthenticationModel model)
        {
            var exists = (await _context.GetListAsync<ELUser>())
                .Where(i => i.Email == model.Email);

            if (!exists.Any())
            {
                return null;
            }

            var db = exists.FirstOrDefault();

            if (BCryptNet.Verify(model.Password, db.Password))
            {
                db.Password = string.Empty;
                return db;
            }

            return null;
        }

        public async Task<bool> RegisterAsync(AuthenticationModel model)
        {
            var exists = (await _context.GetListAsync<ELUser>())
                .Where(i => i.Email == model.Email);

            if(exists.Any())
            {
                return false;
            }

            var user = new ELUser 
            {
                DateCreated = DateTime.Now,
                Email = model.Email,
                Password = BCryptNet.HashPassword(model.Password),
                LastUpdated = DateTime.Now,
            };

            user.SetRoles(ELURoles.User);

            await _context.InsertAsync(user);

            return true;
        }
    }
}
