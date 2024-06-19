using InstHub.Data;
using InstHub.Data.Identity;
using InstHub.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;

namespace InstHub.Controllers
{
    public class GroupsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppIdentityUser> _userManager;
        private readonly IWebHostEnvironment _env;
        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/msword"},
                {".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"},
                {".xml", "application/xml"},
                {".html", "text/html"},
                {".htm", "text/html"},
                {".zip", "application/zip"},
                {".rar", "application/x-rar-compressed"},
                {".7z", "application/x-7z-compressed"},
                {".tar", "application/x-tar"},
                {".gz", "application/gzip"},
                {".mp3", "audio/mpeg"},
                {".wav", "audio/wav"},
                {".mp4", "video/mp4"},
                {".mov", "video/quicktime"},
                {".avi", "video/x-msvideo"},
                {".wmv", "video/x-ms-wmv"},
                {".mkv", "video/x-matroska"},
                {".flv", "video/x-flv"},
                {".webm", "video/webm"},
                {".svg", "image/svg+xml"},
                {".ico", "image/x-icon"},
                {".bmp", "image/bmp"},
                {".rtf", "application/rtf"},
                {".psd", "image/vnd.adobe.photoshop"},
                {".ai", "application/postscript"},
                {".eps", "application/postscript"},
                {".ps", "application/postscript"},
                {".exe", "application/x-msdownload"},
                {".msi", "application/x-msdownload"},
                {".epub", "application/epub+zip"},
                {".mobi", "application/x-mobipocket-ebook"},
                {".azw", "application/vnd.amazon.ebook"},
                {".odp", "application/vnd.oasis.opendocument.presentation"},
                {".ods", "application/vnd.oasis.opendocument.spreadsheet"},
                {".odt", "application/vnd.oasis.opendocument.text"},
                {".ppt", "application/vnd.ms-powerpoint"},
                {".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation"},
                {".apk", "application/vnd.android.package-archive"},
                {".jar", "application/java-archive"},
                {".java", "text/x-java-source"},
                {".class", "application/java-vm"},
                {".c", "text/x-c"},
                {".cpp", "text/x-c"},
                {".h", "text/x-c"},
                {".cs", "text/plain"},
                {".py", "text/x-python"},
                {".js", "application/javascript"},
                {".json", "application/json"},
                {".css", "text/css"},
                {".php", "application/x-httpd-php"},
                {".sql", "application/sql"},
                {".log", "text/plain"},
                {".md", "text/markdown"}
            };
        }

        public class FileViewModel
        {
            public string FileName { get; set; }
            public string Extension { get; set; }
        }

        public class UserFilesViewModel
        {
            public List<FileViewModel> Files { get; set; }
        }

        private string GroupName = "";
        private string folderId = "";

        public GroupsController(ApplicationDbContext context, UserManager<AppIdentityUser> userManager, IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            var userFolderPath = Path.Combine(_env.WebRootPath, "groups", "id3", "files");

            if (file != null && file.Length > 0)
            {
                var filePath = Path.Combine(userFolderPath, file.FileName);
                try
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
                catch (Exception ex)
                {
                    TempData["StatusMessage"] = "Error uploading file.";
                }
            }
            else
            {
                TempData["StatusMessage"] = "No file selected for upload";
            }
            return RedirectToAction("GroupView", new { groupName = GroupName });
        }

        public IActionResult GroupView(string groupName)
        {
            GroupName = groupName;
            ViewData["Title"] = groupName;

            var userFolderPath = Path.Combine(_env.WebRootPath, "groups", "id3", "files");
            var files = Directory.GetFiles(userFolderPath).Select(filePath => new FileViewModel
            {
                FileName = Path.GetFileName(filePath),
                Extension = Path.GetExtension(filePath).ToLower()
            }).ToList();

            var model = new UserFilesViewModel
            {
                Files = files
            };

            return View(model);
        }

        public async Task<IActionResult> Download(string fileName)
        {
            var userFolderPath = Path.Combine(_env.WebRootPath, "groups", "id3", "files");
            var filePath = Path.Combine(userFolderPath, fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File not found");
            }

            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(filePath), fileName);
        }

        public IActionResult Delete(string fileName)
        {
            var userFolderPath = Path.Combine(_env.WebRootPath, "groups", "id3", "files");
            var filePath = Path.Combine(userFolderPath, fileName);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
                TempData["StatusMessage"] = "Файл успешно удален";
            }
            else
            {
                TempData["StatusMessage"] = "Файл не найден";
            }
            return RedirectToAction("GroupView", new { groupName = GroupName });
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

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
