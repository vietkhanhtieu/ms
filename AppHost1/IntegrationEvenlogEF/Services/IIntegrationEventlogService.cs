using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationEvenlogEF.Services
{
    public interface IIntegrationEventlogService
    {
        Task MarkEventAsPushlishAsync(Guid eventId);
        Task MarkEventAsProcessAsync(Guid eventId);
        Task MarkEventAsFailedAsync(Guid eventId);
        Task SaveIntegrationEvent(IntegrationEventLogEntry eventLogEntry);
    }
}
