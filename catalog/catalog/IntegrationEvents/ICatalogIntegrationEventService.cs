using EventBus.Events;

namespace catalog.IntegrationEvents
{
    public interface ICatalogIntegrationEventService
    {
        Task PublishThroughEventBusAsync(IntegrationEvent @event);
        Task SaveEventAndCatalogContextChangesAsync(IntegrationEvent @event);
    }
}
