using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebCrudProject.Auth;
using WebCrudProject.Models;
using WebCrudProject.Service;

namespace WebCrudProject.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Index(string rtnUrl = "/")
        {
            if (User.Identity.IsAuthenticated)
            {
                Redirect(rtnUrl);
            }

            return View(new AuthenticationModel { ReturnUrl = rtnUrl, Email = "admin@webcrud.com", Password = "P@ssw0rd!" });
        }

        [AutoValidateAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> Login(
            [FromForm] AuthenticationModel authenticationModel,
            [FromServices] UserDbService userDbService)
        {
            var result = await userDbService.LoginAsync(new Auth.Models.UserModel 
            {
                UserEmail = authenticationModel.Email,
                UserPassword = authenticationModel.Password,
            });

            if (result != null)
            {
                authenticationModel.ReferenceId = result.UserReference;
                SetCookie(authenticationModel);
                return Redirect(authenticationModel.ReturnUrl);
            }

            return View();
        }

        [AutoValidateAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> Register(
            [FromForm] AuthenticationModel authenticationModel,
            [FromServices] UserDbService userDbService)
        {
            var result = await userDbService.RegisterAsync(new Auth.Models.UserModel
            {
                UserEmail = authenticationModel.Email,
                UserPassword = authenticationModel.Password,
            });

            if (result != null)
            {
                authenticationModel.ReferenceId = result.UserReference;
                SetCookie(authenticationModel);
                return RedirectPermanent(authenticationModel.ReturnUrl);
            }

            return RedirectToAction("Index", new { rtnUrl = authenticationModel.ReturnUrl});
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            Response.Cookies.Delete(SignInMiddelware.SignIngCookie);

            return RedirectToAction("Index");
        }

        [NonAction]
        private void SetCookie(AuthenticationModel authenticationModel)
        {
            authenticationModel.Password = string.Empty;

            Response.Cookies.Append(SignInMiddelware.SignIngCookie,
                JsonConvert.SerializeObject(authenticationModel));
        }
    }
}
