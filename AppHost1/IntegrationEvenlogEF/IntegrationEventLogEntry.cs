using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationEvenlogEF
{
    public class IntegrationEventLogEntry
    {
        public IntegrationEventLogEntry() { }
        [Key]
        public Guid EventId { get; set; }
        public string EventTypeName { get; set; } = string.Empty;
        public string EventTypeShortName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty; 
        public EventStateEnum State { get; set; }
        public int TimesSent { get; set; }
        public DateTime CreationTime { get; set; }

    }
}
