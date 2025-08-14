using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResumeFilterProject.Data;
using ResumeFilterProject.Models;

namespace ResumeFilterProject.Controllers
{
    public class LabelController : Controller
    {
        private readonly ApplicationDbContext _db;

        public LabelController(ApplicationDbContext db)
        {
            _db = db;
        }

        // ✅ List all resumes for labeling (with quick preview)
        [HttpGet]
        public async Task<IActionResult> Index(string q = null, bool onlyUnlabeled = false)
        {
            IQueryable<ResumeUpload> query = _db.Resumes.AsNoTracking().OrderByDescending(r => r.UploadedAt);

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim().ToLower();
                query = query.Where(r =>
                    r.FileName.ToLower().Contains(term) ||
                    (r.ParsedText != null && r.ParsedText.ToLower().Contains(term)) ||
                    (r.Labels != null && r.Labels.ToLower().Contains(term))
                );
            }

            if (onlyUnlabeled)
            {
                query = query.Where(r => string.IsNullOrEmpty(r.Labels));
            }

            var list = await query.ToListAsync();
            return View(list);
        }

        // ✅ Open one resume for labeling
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var r = await _db.Resumes.FindAsync(id);
            if (r == null) return NotFound();

            var vm = new ResumeLabelViewModel
            {
                Id = r.Id,
                FileName = r.FileName,
                FilePath = r.FilePath,
                ParsedText = r.ParsedText,
                Labels = r.Labels
            };
            return View(vm);
        }

        // ✅ Save labels
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ResumeLabelViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var r = await _db.Resumes.FindAsync(vm.Id);
            if (r == null) return NotFound();

            // normalize labels: trim spaces around commas
            r.Labels = NormalizeLabels(vm.Labels);
            await _db.SaveChangesAsync();

            TempData["msg"] = "Labels saved.";
            return RedirectToAction(nameof(Index));
        }

        private static string NormalizeLabels(string labels)
        {
            if (string.IsNullOrWhiteSpace(labels)) return null;
            var parts = labels
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(s => s.Replace("  ", " ").Trim())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct(StringComparer.OrdinalIgnoreCase);
            return string.Join(", ", parts);
        }
    }
}
