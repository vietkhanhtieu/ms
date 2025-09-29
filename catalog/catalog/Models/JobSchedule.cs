using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using catalog.Enumerate;

namespace catalog.Models
{
    public class JobSchedule
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string JobId { get; set; }

        public int Interval { get; set; }
        [Column(TypeName = "int")]
        public JobInternalType JobInternalType { get; set; }
        public Job Job { get; set; }
    }
}
