using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using WebCrudProject.Models;
using WebCrudProject.Service;

namespace WebCrudProject.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index([FromServices] UserDocumentDbService userDocumentDb)
        {
            if (IsAuthenticated)
            {
                var docs = await userDocumentDb.GetUserDocumentsAsync(GetUserInfo()[0]);
                return View(docs);
            }

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}