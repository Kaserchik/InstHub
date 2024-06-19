using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstHub.Controllers
{
    //[Authorize(Roles = "User")]
    public class ConferencesController : Controller
    {
        public IActionResult UserConference()
        {
            return View();
        }

        public IActionResult UserConferences()
        {
            return View();
        }
    }
}
