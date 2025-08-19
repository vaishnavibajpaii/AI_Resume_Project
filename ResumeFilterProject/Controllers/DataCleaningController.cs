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

        // This action saves the confirmed, cleaned data
        [HttpPost]
        public IActionResult SaveChanges(List<ResumeLabel> model)
        {
            foreach (var cleanedItem in model)
            {
                if (cleanedItem.Id == 0)
                {
                    // New record → let DB generate Id
                    _context.ResumeLabels.Add(cleanedItem);
                }
                else
                {
                    // Existing record → update
                    var resumeInDb = _context.ResumeLabels.FirstOrDefault(r => r.Id == cleanedItem.Id);

                    if (resumeInDb != null)
                    {
                        resumeInDb.Name = cleanedItem.Name;
                        resumeInDb.Contact = cleanedItem.Contact;
                        resumeInDb.Skills = cleanedItem.Skills;
                        resumeInDb.Experience = cleanedItem.Experience;
                        resumeInDb.Education = cleanedItem.Education;
                        resumeInDb.Projects = cleanedItem.Projects;

                        _context.ResumeLabels.Update(resumeInDb);
                    }
                }
            }

            _context.SaveChanges();

            // Add a success message to show the user
            TempData["SuccessMessage"] = "Cleaned labels have been saved successfully!";

            return RedirectToAction("Index", "Label");
        }
    }
}
