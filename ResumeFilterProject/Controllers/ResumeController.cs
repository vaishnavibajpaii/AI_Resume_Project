using Microsoft.AspNetCore.Mvc;
using ResumeFilterProject.Data;
using ResumeFilterProject.Models;
using UglyToad.PdfPig;
using Xceed.Words.NET;

namespace ResumeFilterProject.Controllers
{
    public class ResumeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ResumeController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder);

                string filePath = Path.Combine(uploadsFolder, file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                string parsedText = ParseResume(filePath);

                var resume = new ResumeUpload
                {
                    FileName = file.FileName,
                    FilePath = "/uploads/" + file.FileName,
                    ParsedText = parsedText
                };

                _context.Resumes.Add(resume);
                await _context.SaveChangesAsync();

                return RedirectToAction("Upload");
            }

            return View();
        }

        private string ParseResume(string filePath)
        {
            string ext = Path.GetExtension(filePath).ToLower();
            if (ext == ".pdf")
            {
                using (var pdf = PdfDocument.Open(filePath))
                {
                    return string.Join("\n", pdf.GetPages().Select(p => p.Text));
                }
            }
            else if (ext == ".docx")
            {
                using (var doc = DocX.Load(filePath))
                {
                    return doc.Text;
                }
            }
            return "";
        }
    }
}
