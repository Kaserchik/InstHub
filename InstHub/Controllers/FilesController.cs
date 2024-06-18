using InstHub.Data.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace InstHub.Controllers
{
    public class FilesController : Controller
    {
        private readonly UserManager<AppIdentityUser> _userManager;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<FilesController> _logger;

        public class FileViewModel
        {
            public string FileName { get; set; }
            public string Extension { get; set; }
        }

        public class UserFilesViewModel
        {
            public string UserName { get; set; }
            public List<FileViewModel> Files { get; set; }
        }

        public FilesController(UserManager<AppIdentityUser> userManager, IWebHostEnvironment env, ILogger<FilesController> logger)
        {
            _userManager = userManager;
            _env = env;
            _logger = logger;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckAndCreateUserFolder(IFormFile file)
        {

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogError("Unable to load user with ID '{UserId}'", _userManager.GetUserId(User));
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            // Формируем путь к папке пользователя
            var userFolderPath = Path.Combine(_env.WebRootPath, "usersfiles", user.Id);

            // Проверяем наличие папки, если её нет - создаем
            if (!Directory.Exists(userFolderPath))
            {
                Directory.CreateDirectory(userFolderPath);
                _logger.LogInformation("Created directory for user {UserId} at {Path}", user.Id, userFolderPath);
            }

            if (file != null && file.Length > 0)
            {
                var filePath = Path.Combine(userFolderPath, file.FileName);

                _logger.LogInformation("Uploading file {FileName} for user {UserId}", file.FileName, user.Id);

                try
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    TempData["StatusMessage"] = "File uploaded successfully";
                    _logger.LogInformation("File {FileName} uploaded successfully for user {UserId}", file.FileName, user.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error uploading file {FileName} for user {UserId}", file.FileName, user.Id);
                    TempData["StatusMessage"] = "Error uploading file.";
                }
            }
            else
            {
                TempData["StatusMessage"] = "No file selected for upload";
                _logger.LogWarning("No file selected for upload by user {UserId}", user.Id);
            }

            return RedirectToAction("UserFiles");
        }

        public async Task<IActionResult> UserFiles()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var userFolderPath = Path.Combine(_env.WebRootPath, "usersfiles", user.Id);
            var files = Directory.Exists(userFolderPath) ? Directory.GetFiles(userFolderPath) : new string[0];
            var fileViewModels = new List<FileViewModel>();
            foreach (var file in files)
            {
                fileViewModels.Add(new FileViewModel
                {
                    FileName = Path.GetFileName(file),
                    Extension = Path.GetExtension(file).ToLower()
                });
            }

            var model = new UserFilesViewModel
            {
                UserName = user.UserName,
                Files = fileViewModels
            };

            return View(model);
        }
    }
}
