using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using WebCrudProject.Models;
using WebCrudProject.Service;

namespace WebCrudProject.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Index(string? rtnUrl)
        {
            if (User.Identity.IsAuthenticated)
                Redirect(rtnUrl ?? "/");

            return View(new AuthenticationModel { ReturnUrl = rtnUrl });
        }

        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Login([FromForm] AuthenticationModel authenticationModel,
            [FromServices] UserDbService userDbService)
        {

            var result = await userDbService.LoginAsync(new Auth.Models.UserModel 
            {
                Email = authenticationModel.Email,
                Password = authenticationModel.Password,
            });

            if (result)
            {
                SetCookie(authenticationModel);
                return Redirect(authenticationModel.ReturnUrl ?? "/");
            }

            return View();
        }

        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Register([FromForm] AuthenticationModel authenticationModel,
            [FromServices] UserDbService userDbService)
        {

            // Verify user
            // db call
            var result = await userDbService.RegisterAsync(new Auth.Models.UserModel
            {
                Email = authenticationModel.Email,
                Password = authenticationModel.Password,
            });

            if (result)
            {
                SetCookie(authenticationModel);
                return RedirectPermanent(authenticationModel.ReturnUrl ?? "/");
            }

            return Index(authenticationModel.ReturnUrl ?? "Errororor");
        }

        [NonAction]
        private void SetCookie(AuthenticationModel authenticationModel)
        {
            authenticationModel.Password = string.Empty;

            Response.Cookies.Append("cookies", JsonConvert.SerializeObject(authenticationModel));
        }
    }
}
