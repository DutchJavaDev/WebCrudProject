using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebCrudProject.Controllers
{
    public class BaseController : Controller
    {

        public BaseController() 
        {
            ViewBag.IsAuthenticated = IsAuthenticated;
        }

        public bool IsAuthenticated => _Auth();


        [NonAction]
        private bool _Auth()
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
                return new string[] { };


            var user = (ClaimsIdentity)HttpContext.User.Identity;

            var userEmail = user.Claims.Where(i => i.Type.Contains("emailaddress")).FirstOrDefault();
            var userId = user.Claims.Where(i => i.Type.Contains("nameidentifier")).FirstOrDefault();

            return new[] { userId.Value, userEmail.Value };
        }
    }
}
