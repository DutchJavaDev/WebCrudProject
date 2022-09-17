using Microsoft.AspNetCore.Mvc;
using WebCrudProject.Models;

namespace WebCrudProject.Controllers
{
    public class UploadController : BaseController
    {
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = GetUserInfo()[0];
            ///var userFiles = await fileService.ReadFiles(userId);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Files([FromForm] IFormFileCollection files)
        {
            var userId = GetUserInfo()[0];

            if (files == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                // uploading
                //userFiles = await fileService.WriteFiles(userId, files);
            }

            return RedirectToAction("Index");
        }
    }
}
