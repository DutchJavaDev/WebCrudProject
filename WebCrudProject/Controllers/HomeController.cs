using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebCrudProject.Models;
using WebCrudProject.Services.ORM.Interfaces;

namespace WebCrudProject.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromServices] IObjectContext context)
        {
            return View(context == null);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}