using Microsoft.AspNetCore.Mvc;
using WebCrudProject.Service;

namespace WebCrudProject.Controllers
{
    public class UploadController : BaseController
    {
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index([FromForm] IFormFileCollection files, [FromServices] UserFileService fileService)
        {
            var userId = GetUserInfo()[0];

            var uploaded = await fileService.WriteFiles(userId, files);

            return View(uploaded);
        }
    }
}
