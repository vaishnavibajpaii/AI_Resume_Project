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

            // ✅ CORRECTED LINE:
            // This tells the database that the application will provide the Id value.
            modelBuilder.Entity<ResumeLabel>()
                .Property(r => r.Id)
                .ValueGeneratedOnAdd(); // Changed from ValueGeneratedOnAdd()
        }
    }
}