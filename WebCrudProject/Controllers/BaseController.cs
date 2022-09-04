using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebCrudProject.Controllers
{
    public class BaseController : Controller
    {
        public bool IsAuthenticated => Auth();

        [NonAction]
        private bool Auth()
        {
            if(HttpContext == null) return false;

            var id = HttpContext.User.Identity;

            if (id == null) return false;

            return id.IsAuthenticated;
        }

        [NonAction]
        public string[] GetUserInfo()
        {
            if (!IsAuthenticated)
                return Array.Empty<string>();

            var user = HttpContext.User.Identity as ClaimsIdentity;

            var userEmail = user?.Claims.FirstOrDefault(i => i.Type.Contains("emailaddress"));
            var userId = user?.Claims.FirstOrDefault(i => i.Type.Contains("nameidentifier"));

            return new[] { userId.Value, userEmail.Value };
        }
    }
}
