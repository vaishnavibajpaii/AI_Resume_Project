using Microsoft.AspNetCore.Mvc;
using ResumeFilterProject.Services;
using ResumeFilterProject.Data; // Add this

namespace ResumeFilterProject.Controllers
{
    public class ModelTrainingController : Controller
    {
        private readonly ModelTrainingService _trainingService;

        // We get the DbContext to pass it to the service
        public ModelTrainingController(ApplicationDbContext context)
        {
            _trainingService = new ModelTrainingService(context);
        }

        // This action will run the entire pipeline
        public IActionResult Train()
        {
            try
            {
                _trainingService.CreateAndTrainModel();
                TempData["SuccessMessage"] = "AI model training was successful! The model is now updated.";
            }
            catch (System.Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred during training: {ex.Message}";
            }
            // Go back to the main labeling page when done
            return RedirectToAction("Index", "Label");
        }
    }
}