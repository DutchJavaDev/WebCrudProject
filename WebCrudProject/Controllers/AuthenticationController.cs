using Microsoft.AspNetCore.Mvc;
using WebCrudProject.Auth.Models;
using WebCrudProject.Auth.Services.Interfaces;
using WebCrudProject.Services.Email.Interfaces;

namespace WebCrudProject.Controllers
{
    public class AuthenticationController : BaseController
    {
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
                            // Back to login page ??
                            // Bla bla bla :D
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
