using Microsoft.AspNetCore.Mvc;
using ResumeFilterProject.Data;
using ResumeFilterProject.Models;
using ResumeFilterProject.Services;
using System.Collections.Generic;
using System.Linq;

namespace ResumeFilterProject.Controllers
{
    public class DataCleaningController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly DataCleaningService _cleaningService;

        public DataCleaningController(ApplicationDbContext context)
        {
            _context = context;
            _cleaningService = new DataCleaningService();
        }

        // This action receives the raw labeled data from the Label/Index page
        [HttpPost]
        public IActionResult Review(List<ResumeLabel> model)
        {
            // Filter out any rows that the user didn't fill in
            var itemsToProcess = model.Where(item =>
                !string.IsNullOrWhiteSpace(item.Name) ||
                !string.IsNullOrWhiteSpace(item.Skills) ||
                !string.IsNullOrWhiteSpace(item.Experience)
            ).ToList();

            List<ResumeLabel> cleanedModel = new List<ResumeLabel>();
            foreach (var item in itemsToProcess)
            {
                // Use the service to clean each item
                var cleanedItem = _cleaningService.Clean(item);
                cleanedModel.Add(cleanedItem);
            }

            // Send the cleaned data to the Review view
            return View(cleanedModel);
        }

        // In DataCleaningController.cs

        [HttpPost]
        public IActionResult SaveChanges(List<ResumeLabel> model)
        {
            foreach (var cleanedItem in model)
            {
                // First, check if a label with this ID already exists in the database.
                var existingLabel = _context.ResumeLabels.FirstOrDefault(r => r.Id == cleanedItem.Id);

                if (existingLabel == null)
                {
                    // IT DOESN'T EXIST: This is a new record, so we ADD it.
                    // We must create a new object because 'cleanedItem' is not tracked by the context.
                    var newLabel = new ResumeLabel
                    {
                        // NOTE: If your database is set to auto-generate the ID, you should NOT set it here.
                        // If the ID from the main Resume table should be the same, then you MUST set it.
                        // Based on your previous code, you want to preserve the ID.
                        Id = cleanedItem.Id,
                        FileName = cleanedItem.FileName,
                        Name = cleanedItem.Name,
                        Contact = cleanedItem.Contact,
                        Skills = cleanedItem.Skills,
                        Experience = cleanedItem.Experience,
                        Education = cleanedItem.Education,
                        Projects = cleanedItem.Projects
                    };
                    _context.ResumeLabels.Add(newLabel);
                }
                else
                {
                    // IT EXISTS: This is an update, so we modify the existing record.
                    existingLabel.Name = cleanedItem.Name;
                    existingLabel.Contact = cleanedItem.Contact;
                    existingLabel.Skills = cleanedItem.Skills;
                    existingLabel.Experience = cleanedItem.Experience;
                    existingLabel.Education = cleanedItem.Education;
                    existingLabel.Projects = cleanedItem.Projects;
                    _context.ResumeLabels.Update(existingLabel);
                }
            }

            _context.SaveChanges(); // Save all additions and updates to the database at once.

            TempData["SuccessMessage"] = "Cleaned labels have been saved successfully!";
            return RedirectToAction("Index", "Label");
        }
    }
}
