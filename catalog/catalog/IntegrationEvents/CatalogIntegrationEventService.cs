
using EventBus.Abstractions;
using EventBus.Events;
using EventBusRabbitMQ;
using IntegrationEvenlogEF;
using IntegrationEvenlogEF.Services;
using System.Text.Json;

namespace catalog.IntegrationEvents
{
    public class CatalogIntegrationEventService : ICatalogIntegrationEventService 
    {
        private readonly ILogger<CatalogIntegrationEventService> _logger;
        private readonly IIntegrationEventlogService _integrationEventLogService;
        private readonly IEventBus _eventBus;
        public CatalogIntegrationEventService(ILogger<CatalogIntegrationEventService> logger, IIntegrationEventlogService integrationEventLogService, IEventBus eventBus)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _integrationEventLogService = integrationEventLogService ?? throw new ArgumentNullException(nameof(integrationEventLogService));
            _eventBus = eventBus;
        }
        public async Task PublishThroughEventBusAsync<T>(T @event) where T : IntegrationEvent
        {
            try
            {
                _logger.LogInformation("----- Publishing integration event: {IntegrationEventId} from CatalogService - ({@IntegrationEvent})", @event.Id, @event);
                
                await _integrationEventLogService.MarkEventAsProcessAsync(@event.Id);
                //var eventBus = new RabbitMQEventBus();
                await _eventBus.PublishAsync(@event);
                await _integrationEventLogService.MarkEventAsPushlishAsync(@event.Id);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR publishing integration event: {IntegrationEventId} from CatalogService - ({@IntegrationEvent})", @event.Id, @event);
                await _integrationEventLogService.MarkEventAsFailedAsync(@event.Id);
                throw;
            }
        }

        public async Task SaveEventAndCatalogContextChangesAsync(IntegrationEvent @event)
        {
            try
            {
                var runtimeType = @event.GetType();
                var eventLogEntry = new IntegrationEventLogEntry
                {
                    Content = JsonSerializer.Serialize(@event, runtimeType),
                    EventId = @event.Id,
                    CreationTime = @event.CreationDate,
                    EventTypeName = @event.GetType().FullName,
                    State = EventStateEnum.NotPublished,
                    TimesSent = 0,
                    EventTypeShortName = runtimeType.Name,
                };
                await _integrationEventLogService.SaveIntegrationEvent(eventLogEntry);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR saving integration event: {IntegrationEventId} from CatalogService - ({@IntegrationEvent})", @event.Id, @event);
                throw;
            }
        }
    }
}
