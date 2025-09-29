using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using catalog.Enumerate;

namespace catalog.Models
{
    public class Job
    {
        [Key]
        public string Id { get; set; }
        [Column(TypeName = "int")]
        public JobType Type { get; set; }
        [Column(TypeName = "int")]
        public JobStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public long NextRunTime { get; set; }
        public JobSchedule JobSchedule { get; set; }
    }
}
