using Microsoft.AspNetCore.Mvc;

namespace InstHub.Controllers
{
    public class FilesController : Controller
    {
        public IActionResult UserFiles()
        {
            return View();
        }
    }
}
