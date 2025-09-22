using ScheduleJob.Enumn;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskStatus = ScheduleJob.Enumn.TaskStatus;

namespace ScheduleJob.Model
{
    public class Job
    {
        [Key]
        public string Id { get; set; }
        [Column(TypeName = "int")]
        public TaskType Type { get; set; }
        [Column(TypeName = "int")]
        public TaskStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public long NextRunTime { get; set; }
        public TaskSchedule TaskSchedule { get; set; }
    }
}
