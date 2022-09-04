using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebCrudProject.Auth;
using WebCrudProject.Auth.Models;
using WebCrudProject.Models;
using WebCrudProject.Service;

namespace WebCrudProject.Controllers
{
    public class AuthController : BaseController
    {
        public IActionResult Index(string rtnUrl = "/")
        {
            if (IsAuthenticated)
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
            SetCookie(); 

            var result = await userDbService.LoginAsync(new UserModel
            {
                UserEmail = authenticationModel.Email,
                UserPassword = authenticationModel.Password,
            });

            return VerifyResult(authenticationModel, result);
        }

        private IActionResult VerifyResult(AuthenticationModel authenticationModel, UserModel result)
        {
            if (result != null)
            {
                authenticationModel.ReferenceId = result.UserReference;
                SetCookie(authenticationModel);
                return Redirect(authenticationModel.ReturnUrl);
            }

            return RedirectToAction("Index", new { rtnUrl = authenticationModel.ReturnUrl });
        }

        [AutoValidateAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> Register(
            [FromForm] AuthenticationModel authenticationModel,
            [FromServices] UserDbService userDbService)
        {
            ///SetCookie();

;            var result = await userDbService.RegisterAsync(new UserModel
            {
                UserEmail = authenticationModel.Email,
                UserPassword = authenticationModel.Password,
                UserReference = Guid.NewGuid().ToString()
            });

            return VerifyResult(authenticationModel, result);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            SetCookie();
            return RedirectToAction("Index");
        }

        [NonAction]
        private void SetCookie(AuthenticationModel authenticationModel = null)
        {
            if (authenticationModel == null)
            {
                Response.Cookies.Delete(SignInMiddelware.SignIngCookie);
            }
            else
            {
                authenticationModel.Password = string.Empty;

                Response.Cookies.Append(SignInMiddelware.SignIngCookie,
                    JsonConvert.SerializeObject(authenticationModel));
            }
        }
    }
}
