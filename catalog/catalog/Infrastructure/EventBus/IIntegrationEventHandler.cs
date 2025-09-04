namespace catalog.Infrastructure.EventBus
{
    public interface IIntegrationEventHandler<T>
    {
        Task Handle(T @event);
    }
}
