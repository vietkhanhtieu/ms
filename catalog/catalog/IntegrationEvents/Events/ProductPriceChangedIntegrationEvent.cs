using EventBus.Events;

namespace catalog.IntegrationEvents.Events
{
    public class ProductPriceChangedIntegrationEvent : IntegrationEvent
    {
        public ProductPriceChangedIntegrationEvent(int productId, decimal oldPrice, decimal newPrice)
        {
            ProductId = productId;
            OldPrice = oldPrice;
            NewPrice = newPrice;
        }
        public int ProductId { get; }
        public decimal OldPrice { get; }
        public decimal NewPrice { get; }
    }
}
