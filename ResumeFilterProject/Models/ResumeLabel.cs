namespace ResumeFilterProject.Models
{
   
    public class ResumeLabel
    {
        public int Id { get; set; }
        public string FileName { get; set; }

        // Fields to label
        public string Name { get; set; }
        public string Contact { get; set; }
        public string Skills { get; set; }
        public string Experience { get; set; }
        public string Education { get; set; }
        public string Projects { get; set; }
    }

}
