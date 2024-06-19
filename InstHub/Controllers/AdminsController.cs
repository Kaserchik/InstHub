using Microsoft.AspNetCore.Mvc;

namespace InstHub.Controllers
{
    public class AdminsController : Controller
    {
        public IActionResult Admin()
        {
            return View();
        }
    }
}
