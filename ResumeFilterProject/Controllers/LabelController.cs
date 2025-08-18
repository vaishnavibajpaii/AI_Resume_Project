using Microsoft.AspNetCore.Mvc;
using ResumeFilterProject.Data;
using ResumeFilterProject.Models;
using System.Collections.Generic;
using System.Linq;

namespace ResumeFilterProject.Controllers
{
    public class LabelController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LabelController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ FIX: Load ALL resumes and join them with any existing labels.
        public IActionResult Index()
        {
            // Get all the main resumes first (assuming you have a 'Resumes' table/DbSet)
            var allResumes = _context.Resumes.ToList();
            var existingLabels = _context.ResumeLabels.ToDictionary(l => l.Id);

            var model = allResumes.Select(r =>
            {
                // If a label already exists for this resume, use it.
                if (existingLabels.TryGetValue(r.Id, out var label))
                {
                    return label;
                }

                // Otherwise, create a NEW, empty label object for the view.
                return new ResumeLabel { Id = r.Id, FileName = r.FileName };

            }).ToList();

            return View(model);
        }

        [HttpPost]
        public IActionResult SaveLabels(List<ResumeLabel> model)
        {
            foreach (var item in model)
            {
                // 🛑 PROBLEM WAS HERE: This only looked for existing resumes.
                var resumeInDb = _context.ResumeLabels.FirstOrDefault(r => r.Id == item.Id);

                // ✅ FIX: If resume label exists, UPDATE it.
                if (resumeInDb != null)
                {
                    resumeInDb.Name = item.Name;
                    resumeInDb.Contact = item.Contact;
                    resumeInDb.Skills = item.Skills;
                    resumeInDb.Experience = item.Experience;
                    resumeInDb.Education = item.Education;
                    resumeInDb.Projects = item.Projects;
                    _context.Update(resumeInDb);
                }
                // ✅ FIX: If resume label does NOT exist, CREATE it.
                else
                {
                    // We only add if there is some data to save
                    if (!string.IsNullOrWhiteSpace(item.Name) || !string.IsNullOrWhiteSpace(item.Skills))
                    {
                        var newLabel = new ResumeLabel
                        {
                            //Id = item.Id,
                            FileName = item.FileName, // Make sure FileName is passed from the form
                            Name = item.Name,
                            Contact = item.Contact,
                            Skills = item.Skills,
                            Experience = item.Experience,
                            Education = item.Education,
                            Projects = item.Projects
                        };
                        _context.Add(newLabel);
                    }
                }
            }
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // Your Edit actions are likely fine, but the logic above is the main fix.
        // ... (keep your existing Edit GET and POST methods here)
    }
}