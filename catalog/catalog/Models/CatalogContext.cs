using IntegrationEvenlogEF;
using Microsoft.EntityFrameworkCore;

namespace catalog.Models
{
    public class CatalogContext: DbContext
    {
        public CatalogContext(DbContextOptions<CatalogContext> options) : base(options)
        {
        }

        public DbSet<CatalogItem> CatalogItems { get; set; }

        public DbSet<IntegrationEventLogEntry> integrationEventLogEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           
        }
    }
}
