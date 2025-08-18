using ResumeFilterProject.Models;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ResumeFilterProject.Services
{
    public class DataCleaningService
    {
        public ResumeLabel Clean(ResumeLabel label)
        {
            if (label == null) return null;

            // Step 1 & 3: Handle Missing Data and Structural Errors (Whitespace)
            label.Name = label.Name?.Trim();
            label.Contact = label.Contact?.Trim();
            label.Skills = label.Skills?.Trim();
            label.Experience = label.Experience?.Trim();
            label.Education = label.Education?.Trim();
            label.Projects = label.Projects?.Trim();

            // Step 3: Standardize Data (Casing and Replacements)
            if (!string.IsNullOrEmpty(label.Skills))
            {
                // Convert to lowercase for consistency
                label.Skills = label.Skills.ToLower();
                // Example: Replace variations
                label.Skills = label.Skills.Replace("c-sharp", "c#");
            }

            if (!string.IsNullOrEmpty(label.Education))
            {
                label.Education = label.Education.ToLower();
                // Standardize common terms
                var educationMappings = new Dictionary<string, string>
                {
                    { "b. tech", "b.tech" },
                    { "bachelor of technology", "b.tech" },
                    { "m. tech", "m.tech" },
                    { "master of technology", "m.tech" }
                };

                foreach (var map in educationMappings)
                {
                    label.Education = label.Education.Replace(map.Key, map.Value);
                }
            }

            // Example: Clean phone numbers to only digits
            if (!string.IsNullOrEmpty(label.Contact))
            {
                label.Contact = Regex.Replace(label.Contact, @"\D", "");
            }

            // Step 1 Continued: Handle nulls for non-essential fields
            if (string.IsNullOrWhiteSpace(label.Projects))
            {
                label.Projects = null; // Enforce NULL instead of empty strings
            }

            return label;
        }
    }
}