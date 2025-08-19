using Microsoft.ML;
using ResumeFilterProject.Data;
using ResumeFilterProject.Models;
using System.Collections.Generic;
using System.Linq;

namespace ResumeFilterProject.Services
{
    public class ModelTrainingService
    {
        private readonly ApplicationDbContext _context;
        private readonly DataCleaningService _cleaningService;
        // This defines where the trained model will be saved
        private static readonly string _modelPath = "trained_model.zip";

        public ModelTrainingService(ApplicationDbContext context)
        {
            _context = context;
            _cleaningService = new DataCleaningService();
        }

        public void CreateAndTrainModel()
        {
            // --- Automated Pipeline Starts Here ---
            var mlContext = new MLContext();

            // 1. LOAD DATA from your database
            var rawData = _context.ResumeLabels
                .Where(r => !string.IsNullOrEmpty(r.Skills)) // Use only labeled data
                .ToList();

            if (rawData.Count < 5) // Need at least a few examples to learn
            {
                throw new System.Exception("Not enough data to train. Please label at least 5 resumes with skills.");
            }

            // 2. PREPROCESS/CLEAN DATA using your existing service
            var cleanedData = rawData.Select(label => _cleaningService.Clean(label)).ToList();
            IDataView trainingData = mlContext.Data.LoadFromEnumerable(cleanedData);

            // 3. DEFINE TRAINING PIPELINE
            // We'll train the model to predict the "Skills" based on the text in the "Experience" field.
            var pipeline = mlContext.Transforms.Conversion.MapValueToKey("Label", nameof(ResumeLabel.Skills))
                .Append(mlContext.Transforms.Text.FeaturizeText("Features", nameof(ResumeLabel.Experience)))
                .Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy("Label", "Features"))
                .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            // 4. TRAIN THE MODEL
            var model = pipeline.Fit(trainingData);

            // 5. SAVE THE MODEL to the file defined in _modelPath
            mlContext.Model.Save(model, trainingData.Schema, _modelPath);
        }
    }
}