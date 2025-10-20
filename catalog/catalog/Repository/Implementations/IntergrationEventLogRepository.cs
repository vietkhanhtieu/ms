using catalog.Models;
using catalog.Repository.Interfaces;
using IntegrationEvenlogEF;

namespace catalog.Repository.Implementations
{
    public class IntergrationEventLogRepository : IIntergrationEventLogRepository
    {
        private readonly CatalogContext _context;

        public IntergrationEventLogRepository(CatalogContext context)
        {
            _context = context;
        }

        public async Task<List<IntegrationEventLogEntry>> GetEventFailAll()
        {
            return _context.IntegrationEventLogEntries.Where(x => x.State == EventStateEnum.PublishedFailed).ToList();
        }


    }
}
