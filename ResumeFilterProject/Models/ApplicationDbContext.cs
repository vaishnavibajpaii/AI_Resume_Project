using Microsoft.EntityFrameworkCore;
using ResumeFilterProject.Models;

namespace ResumeFilterProject.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<ResumeUpload> Resumes { get; set; }
        public DbSet<ResumeLabel> ResumeLabels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ✅ THIS IS THE FIX:
            // This line now correctly tells the database that your application
            // will provide the Id value, and it should NOT generate one itself.
            modelBuilder.Entity<ResumeLabel>()
                .Property(r => r.Id)
                .ValueGeneratedNever(); // Changed from .ValueGeneratedOnAdd()
        }
    }
}