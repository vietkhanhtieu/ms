using IntegrationEvenlogEF;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace catalog.Models
{
    public class CatalogContext: DbContext
    {
        public CatalogContext(DbContextOptions<CatalogContext> options) : base(options)
        {
        }

        public DbSet<CatalogItem> CatalogItems { get; set; }

        public DbSet<IntegrationEventLogEntry> IntegrationEventLogEntries { get; set; }

        public DbSet<Job> Jobs { get; set; }
        public DbSet<JobSchedule> JobSchedules { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Job>(e =>
            {
                e.HasKey(t => t.Id);
                e.Property(t => t.Id).IsRequired();
                e.Property(t => t.Type).IsRequired();
                e.HasOne(t => t.JobSchedule)
                 .WithOne(ts => ts.Job)
                 .HasForeignKey<JobSchedule>(ts => ts.JobId)
                 .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<JobSchedule>(e =>
            {
                e.HasKey(t => t.Id);
            });
        }
    }
}
