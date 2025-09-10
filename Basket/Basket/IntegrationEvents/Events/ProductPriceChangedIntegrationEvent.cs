using EventBus.Events;

namespace Basket.IntegrationEvents.Events
{
    public class ProductPriceChangedIntegrationEvent : IntegrationEvent
    {
        public int ProductId { get; set; }
        public decimal OldPrice { get; set; }
        public decimal NewPrice { get; set; }
        public DateTime OccurredOn { get; set; }
    }
}
