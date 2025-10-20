using catalog.IntegrationEvents.Events;
using catalog.Repository.Interfaces;
using EventBus.Abstractions;
using IntegrationEvenlogEF.Services;
using System.Text.Json;

namespace catalog.JobExcutor
{
    public class EventExecutor : IJobExecutor
    {
        private readonly ILogger<SyncStockExecutor> _logger;
        private readonly IIntergrationEventLogRepository _intergrationEventLogRepository;
        private readonly IIntegrationEventlogService _integrationEventLogService;
        private readonly IEventBus _eventBus;

        static readonly Dictionary<string, Type> _eventTypeMap = new(StringComparer.OrdinalIgnoreCase)
        {
            { "ProductPriceChangedIntegrationEvent", typeof(ProductPriceChangedIntegrationEvent) },
        };

        public EventExecutor(ILogger<SyncStockExecutor> logger, IIntergrationEventLogRepository intergrationEventLogRepository, IIntegrationEventlogService integrationEventLogService,
            IEventBus eventBus)
        {
            _logger = logger;
            _intergrationEventLogRepository = intergrationEventLogRepository;
            _integrationEventLogService = integrationEventLogService;
            _eventBus = eventBus;
        }

        public async System.Threading.Tasks.Task ExecutorAsync(JobBase task)
        {
            _logger.LogInformation($"Starting execute Event for Task ID: {task.Id}");
            var eventFails = await _intergrationEventLogRepository.GetEventFailAll();
            if (eventFails.Any())
            {
                foreach(var item in eventFails)
                {
                    var evtType = ResolveEventType(item.EventTypeName, item.EventTypeShortName);
                    var even = JsonSerializer.Deserialize(item.Content, evtType);
                    await _eventBus.PublishAsync(even);
                    await _integrationEventLogService.MarkEventAsPushlishAsync(item.EventId);
                }
            }
        }

        private static Type ResolveEventType(string eventTypeName, string shortName)
        {
            var type = Type.GetType(eventTypeName, throwOnError: false, ignoreCase: false);
            if (type != null) return type;

            if (!string.IsNullOrWhiteSpace(shortName) && _eventTypeMap.TryGetValue(shortName, out var t))
                return t;

            throw new InvalidOperationException($"Cannot resolve event type: {eventTypeName} / {shortName}");
        }
    }
}
