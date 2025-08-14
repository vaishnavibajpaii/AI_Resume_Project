using System;
using System.ComponentModel.DataAnnotations;

namespace ResumeFilterProject.Models
{
    public class ResumeUpload
    {
        public int Id { get; set; }

        [Required]
        public string FileName { get; set; }

        public string FilePath { get; set; }

        public string ParsedText { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.Now;

        
        public string? Labels { get; set; }
    }
}
