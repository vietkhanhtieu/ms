using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationEvenlogEF.Services
{
    public class IntegrationEventLogServices<TContext> : IIntegrationEventlogService where TContext : DbContext
    {
        private readonly TContext _context;

        public IntegrationEventLogServices(TContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task MarkEventAsPushlishAsync(Guid eventId)
        {
            await UpdateEventState(eventId, EventStateEnum.Published);
        }

        public Task MarkEventAsProcessAsync(Guid eventId)
        {
            return UpdateEventState(eventId, EventStateEnum.InProgress);
        }

        public Task MarkEventAsFailedAsync(Guid eventId)
        {
            return UpdateEventState(eventId, EventStateEnum.PublishedFailed);
        }

        public async Task SaveIntegrationEvent(IntegrationEventLogEntry eventLogEntry)
        {
            if (eventLogEntry == null)
            {
                throw new ArgumentNullException(nameof(eventLogEntry));
            }
            _context.Set<IntegrationEventLogEntry>().Add(eventLogEntry);
            await _context.SaveChangesAsync();
        }

        private Task UpdateEventState(Guid eventId, EventStateEnum eventState)
        {
            var log = _context.Set<IntegrationEventLogEntry>().Single(ie => ie.EventId == eventId);
            log.State = eventState;
            if (eventState == EventStateEnum.InProgress)
            {
                log.TimesSent++;
            }
            _context.Update(log);
            return _context.SaveChangesAsync();
        }
    }
}
