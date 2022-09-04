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

        [HttpGet]
        public async Task<IActionResult> Update(int number, 
            [FromServices] UserDocumentDbService userDocumentDb) 
        {
            var model = await userDocumentDb.GetUserDocumentByNumberAsync(number);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDocument([FromForm] UserDocument document,
            [FromServices] UserDocumentDbService userDocumentDb)
        {
            var result =  await userDocumentDb.UpdateDocumentAsync(document);

            if(!result)
            {
                return BadRequest();
            }
            return Redirect("/");
        }

        [HttpGet]
        public async Task<IActionResult> DeleteDocument(int number, [FromServices] UserDocumentDbService userDocumentDb) 
        {
            await userDocumentDb.DeleteDocumentAsync(number);

            return Redirect("/");
        }
    }
}
