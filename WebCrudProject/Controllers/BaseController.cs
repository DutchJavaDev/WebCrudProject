using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using WebCrudProject.Auth;
using WebCrudProject.Auth.Models;
using WebCrudProject.Models;

namespace WebCrudProject.Controllers
{
    public class BaseController : Controller
    {
        public DefaultViewModel DefaultViewModel()
        {
            return new ()
            {
                IsAuthenticated = UserAuthenticated(),
                Email = GetClaimTypeValue(ClaimTypes.Email),
                IsAdmin = HasRole(ELURoles.Admin)
            };
        }

        public void SetSessionId(string id)
        {
            Request.HttpContext.Session.Set(AuthenticationMiddelWare.SessionID, Encoding.UTF8.GetBytes(id));
        }

        public bool HasRole(ELURoles role)
        {
            return User.IsInRole(role.ToString());
        }

        public string GetClaimTypeValue(string type)
        {
            if (User.Identity == null || !User.Identities.Any())
                return string.Empty;

            var claim = User.Claims.Where(i => i.Type == type).FirstOrDefault();
        
            if(claim == null)
            {
                return string.Empty;
            }

            return claim.Value;
        }
        public bool UserAuthenticated()
        {
            if (User.Identity == null)
                return false;

            return User.Identity.IsAuthenticated;
        }
    }
}
