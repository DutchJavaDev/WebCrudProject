using Microsoft.AspNetCore.Mvc;
using WebCrudProject.Models;
using WebCrudProject.Service;

namespace WebCrudProject.Controllers
{
    public class UploadController : BaseController
    {
        [HttpGet]
        public async Task<IActionResult> Index([FromServices] UserFileService fileService)
        {
            var userId = GetUserInfo()[0];
            var userFiles = await fileService.ReadFiles(userId);
            return View(userFiles);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Files([FromForm] IFormFileCollection files, [FromServices] UserFileService fileService)
        {
            var userId = GetUserInfo()[0];
            List<UserFileModel> userFiles;

            if (files == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                // uploading
                userFiles = await fileService.WriteFiles(userId, files);
            }

            return RedirectToAction("Index");
        }
    }
}
