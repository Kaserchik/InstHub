using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InstHub.Controllers
{
    public class GroupsController : Controller
    {
        //[Authorize(Roles = "User")]
        public IActionResult CreateGroupView()
        {
            return View();
        }

        public IActionResult UserGroups()
        {
            return View();
        }
    }
}
