using System.Text;
using Microsoft.AspNetCore.Mvc;
using WebCrudProject.Auth.Models;
using WebCrudProject.Auth.Services.Interfaces;
using WebCrudProject.Services.Email.Interfaces;

namespace WebCrudProject.Controllers
{
    public class AuthenticationController : BaseController
    {
        [HttpGet]
        public async Task<IActionResult> Logout([FromServices] ISessionService sessionService)
        {
            if(TryGetSessionId(HttpContext, out var id))
            {
                var _session = await sessionService.ResolveSessionAsync(id);
                await sessionService.DeleteSessionAsync(_session.Item2.Id);
            }

            return RedirectToAction("Index","Home");
        }

        private static bool TryGetSessionId(HttpContext request, out string sessionId)
        {
            byte[]? byteArrayResult;

            var boolResult = request.Session.TryGetValue("", out byteArrayResult);

            if (boolResult 
                && byteArrayResult != null)
            {
                sessionId = Encoding.UTF8.GetString(byteArrayResult);
            }
            else
            {
                sessionId = string.Empty;
            }

            return boolResult;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(
            [FromForm] AuthenticationModel authenticationModel,
            [FromServices] IAuthenticationService authentication,
            [FromServices] ISessionService session,
            [FromServices] IELMailService mail)
        {
            if (ModelState.IsValid)
            {
                switch (authenticationModel.AuthenticationType)
                {
                    case UserAuthenticationState.Login:
                        var user = await authentication.LoginAsync(authenticationModel);
                        if (user == null)
                        {
                            ModelState.AddModelError("Login fail", "Login failed");
                        }
                        else
                        {
                            var sessionId = await session.CreateSessionAsync(user);
                            SetSessionId(sessionId);
                        }
                        break;

                    case UserAuthenticationState.Register:
                        var result = await authentication.RegisterAsync(authenticationModel);

                        if (result)
                        {
                            user = await authentication.LoginAsync(authenticationModel);
                            await mail.SendAccountRegistrationAsyn(user);
                            var sessionId = await session.CreateSessionAsync(user);
                            SetSessionId(sessionId);
                        }
                        else
                        {
                            ModelState.AddModelError("register error", "User already exists");
                        }
                        
                        break;

                    default: break;
                }
            }

            return RedirectToAction("Index","Home");
        }
    }
}
