using Microsoft.AspNetCore.Mvc;

namespace InstHub.Controllers
{
    public class GroupsController : Controller
    {
        public IActionResult UserGroups()
        {
            return View();
        }
    }
}
