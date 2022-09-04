using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebCrudProject.Models;
using WebCrudProject.Service;

namespace WebCrudProject.Controllers
{
    public class DocumentController : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View();
        }


        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Create([FromForm] UserDocument userDocument,
            [FromServices] UserDocumentDbService userDocumentDb)
        {
            var user = GetUserInfo()[0];

            var result = await userDocumentDb.CreateDocumentAsync(user, userDocument);

            return result ? RedirectToAction("Index") : BadRequest();
        }

        [NonAction]
        public string[] GetUserInfo()
        {
            var user = (ClaimsIdentity)HttpContext.User.Identity;

            var userEmail = user.Claims.Where(i => i.Type.Contains("emailaddress")).FirstOrDefault();
            var userId = user.Claims.Where(i => i.Type.Contains("nameidentifier")).FirstOrDefault();

            return new[] { userId.Value, userEmail.Value };
        }
    }
}
