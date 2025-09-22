using ScheduleJob.Enumn;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleJob.Model
{
    public class TaskSchedule
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string TaskId { get; set; }

        public int Interval { get; set; }
        [Column(TypeName = "int")]
        public TaskInternalType TaskInternalType { get; set; }
        public Job Job { get; set; }
    }
}
