namespace Basket.Infractructure.EvenBus
{
    public interface IIntegrationEventHandler<T>
    {
        Task Handle(T @event);
    }
}
