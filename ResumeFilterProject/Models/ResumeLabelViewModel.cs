using System.ComponentModel.DataAnnotations;

namespace ResumeFilterProject.Models
{
    public class ResumeLabelViewModel
    {
        // DB key
        public int Id { get; set; }

        // Read-only display
        public string FileName { get; set; }
        public string FilePath { get; set; }

        // Extracted text (preview / context for human labeler)
        public string ParsedText { get; set; }

        // Editable labels
        [Display(Name = "Labels (comma-separated)")]
        public string Labels { get; set; }
    }
}
