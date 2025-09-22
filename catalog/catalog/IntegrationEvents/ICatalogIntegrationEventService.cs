using EventBus.Events;

namespace catalog.IntegrationEvents
{
    public interface ICatalogIntegrationEventService
    {
        Task PublishThroughEventBusAsync<T>(T @event) where T : IntegrationEvent;
        Task SaveEventAndCatalogContextChangesAsync(IntegrationEvent @event);
    }
}
