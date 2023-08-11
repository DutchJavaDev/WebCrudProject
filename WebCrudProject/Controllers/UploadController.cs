using Microsoft.AspNetCore.Mvc;
using WebCrudProject.Models;

namespace WebCrudProject.Controllers
{
    public class UploadController : BaseController
    {
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Files([FromForm] IFormFileCollection files)
        {

            return RedirectToAction("Index");
        }
    }
}
