using Microsoft.AspNetCore.Mvc;

namespace InstHub.Controllers
{
    public class ConferencesController : Controller
    {
        public IActionResult UserConferences()
        {
            return View();
        }
    }
}
