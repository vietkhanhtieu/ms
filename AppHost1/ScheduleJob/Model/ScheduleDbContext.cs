using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleJob.Model
{
    public class ScheduleDbContext : DbContext
    {
        public ScheduleDbContext(DbContextOptions<ScheduleDbContext> options) : base(options)
        {
        }

        public DbSet<Job> Jobs { get; set; }
        public DbSet<TaskSchedule> TaskSchedules { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Job>(e =>
            {
                e.HasKey(t => t.Id);
                e.Property(t => t.Id).IsRequired();
                e.Property(t => t.Type).IsRequired();
                e.HasOne(t => t.TaskSchedule)
                 .WithOne(ts => ts.Job)
                 .HasForeignKey<TaskSchedule>(ts => ts.TaskId)
                 .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<TaskSchedule>(e =>
            {
                e.HasKey(t => t.Id);
            });
        }
    }
}
