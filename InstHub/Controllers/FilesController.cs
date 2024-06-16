using InstHub.Data.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace InstHub.Controllers
{
    public class FilesController : Controller
    {
        private readonly UserManager<AppIdentityUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public FilesController(UserManager<AppIdentityUser> userManager, IWebHostEnvironment env)
        {
            _userManager = userManager;
            _env = env;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckAndCreateUserFolder(IFormFile file)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            // Формируем путь к папке пользователя
            var userFolderPath = Path.Combine(_env.WebRootPath, "usersfiles", user.Id);

            // Проверяем наличие папки, если её нет - создаем
            if (!Directory.Exists(userFolderPath))
            {
                Directory.CreateDirectory(userFolderPath);
            }

            if (file != null && file.Length > 0)
            {
                var filePath = Path.Combine(userFolderPath, file.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                TempData["StatusMessage"] = "Folder checked/created and file uploaded successfully";
            }
            else
            {
                TempData["StatusMessage"] = "Folder checked/created but no file selected for upload";
            }

            TempData["StatusMessage"] = "Folder checked/created successfully";

            return RedirectToAction("UserFiles");
        }

        public IActionResult UserFiles()
        {
            return View();
        }
    }
}
