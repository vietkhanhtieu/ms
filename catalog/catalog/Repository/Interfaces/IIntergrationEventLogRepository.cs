using IntegrationEvenlogEF;

namespace catalog.Repository.Interfaces
{
    public interface IIntergrationEventLogRepository
    {
        Task<List<IntegrationEventLogEntry>> GetEventFailAll();
    }
}
