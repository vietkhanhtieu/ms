namespace Basket.Infractructure.EvenBus
{
    public interface IEventBus
    {
        Task PublishAsync<T>(T @event);

        Task SubscribeAsync<T, TH>()
            where T : class
            where TH : IIntegrationEventHandler<T>;

        void Unsubscribe<T, TH>()
            where T : class
            where TH : IIntegrationEventHandler<T>;
    }
}
