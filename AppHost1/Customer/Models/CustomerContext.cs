using Microsoft.EntityFrameworkCore;

namespace Customer.Models
{
    public class CustomerContext : DbContext
    {
        public CustomerContext(DbContextOptions<CustomerContext> options) : base(options) { }
        public DbSet<CustomerEntity> Customers { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<CustomerEntity>(e =>
            {
                e.HasKey(e => e.Id);
                e.Property(e => e.Name).IsRequired();
                e.Property(e =>e.Email).IsRequired();
                e.Property(e=>e.Phone).IsRequired();
            });


        }
    }
}
