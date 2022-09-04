using Microsoft.AspNetCore.Mvc;
using WebCrudProject.Models;
using WebCrudProject.Service;

namespace WebCrudProject.Controllers
{
    public class DocumentController : BaseController
    {
        public IActionResult Index()
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
    }
}
