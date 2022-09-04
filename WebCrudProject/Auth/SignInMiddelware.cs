using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Security.Claims;
using WebCrudProject.Models;

namespace WebCrudProject.Auth
{
    public static class SignInMiddelware
    {

        public static readonly string SignIngCookie = "_sgnCku343=";

        /// <summary>
        /// Checks if there is a signin cookie, if so redirect to previous url or else to /, else send to register & login
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <param name="next">Func<Task></param>
        /// <returns></returns>
        public static async Task CheckSignIn(HttpContext context, Func<Task> next)
        {
            if (!context.User.Identity.IsAuthenticated && !context.Request.Path.Value.Contains("Auth"))
            {
                var hasCookie = context.Request.Cookies.TryGetValue(SignIngCookie, out var userAuth);

                if (hasCookie)
                {
                    var model = JsonConvert.DeserializeObject<AuthenticationModel>(userAuth);
                    var claims = new[] {
                    new Claim(ClaimTypes.Email, model.Email),
                    new Claim(ClaimTypes.NameIdentifier, model.ReferenceId)
                };
                    ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    ClaimsPrincipal principal = new ClaimsPrincipal(identity);
                    context.User = principal;
                }
                else
                {
                    var path = context.Request.Path.ToString();

                    context.Response.Redirect($"/Auth/Index?rtnUrl={path}");
                }

            }

            await next();
        }
    }
}
