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
            var model = new AuthenticationModel 
            {
                Email = "Boris",
                Password = "KWende",
                ReturnUrl = "asdasdsa",
                DateCreated = DateTime.Now,
                LastUpdated = DateTime.Now
            };

            await context.InsertAsync(model);

            var db = await context.GetListAsync<AuthenticationModel>();

            return View(db.Last());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}