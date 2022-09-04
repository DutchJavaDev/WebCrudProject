using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using WebCrudProject.Models;
using WebCrudProject.Service;

namespace WebCrudProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index([FromServices] UserDocumentDbService userDocumentDb)
        {
            var docs = await userDocumentDb.GetUserDocumentsAsync(GetUserInfo()[0]);

            return View(docs);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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