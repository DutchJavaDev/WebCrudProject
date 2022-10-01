using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using WebCrudProject.Auth;
using WebCrudProject.Auth.Models;
using WebCrudProject.Models;

namespace WebCrudProject.Controllers
{
    public class BaseController : Controller
    {
        private readonly string CLIENT_ERROR_ID = "_=sfdk23";

        public DefaultViewModel DefaultViewModel()
        {
            var _erross = ResolveClientErrors();

            ClearClientErrors();
            return new()
            {
                IsAuthenticated = UserAuthenticated(),
                Email = GetClaimTypeValue(ClaimTypes.Email),
                IsAdmin = HasRole(ELURoles.Admin),
                ErrorList = _erross
            };
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);

            var errorList = new List<string>();

            var errors = ModelState.SelectMany(i => i.Value.Errors);

            foreach (var error in errors)
            {
                errorList.Add($"{error.ErrorMessage}");
            }

            SetClienErrors(errorList);
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
        private bool UserAuthenticated()
        {
            if (User.Identity == null)
                return false;

            return User.Identity.IsAuthenticated;
        }
        private void ClearClientErrors()
        {
            Request.HttpContext.Session.Set(CLIENT_ERROR_ID, new byte[] { });
        }
        private void SetClienErrors(List<string> errors)
        {
            var _json = JsonSerializer.Serialize(errors);
            Request.HttpContext.Session.Set(CLIENT_ERROR_ID,Encoding.UTF8.GetBytes(_json));
        }
        private IEnumerable<string> ResolveClientErrors()
        {
            var _bytes = Request.HttpContext.Session.Get(CLIENT_ERROR_ID);

            if(_bytes == null)
                return Enumerable.Empty<string>();

            var str = Encoding.UTF8.GetString(_bytes);

            return JsonSerializer.Deserialize<List<string>>(str);
        }
    }
}
