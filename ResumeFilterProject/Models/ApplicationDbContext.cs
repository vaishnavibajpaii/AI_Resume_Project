using Microsoft.EntityFrameworkCore;
using ResumeFilterProject.Models;

namespace ResumeFilterProject.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<ResumeUpload> Resumes { get; set; }
    }
}
