namespace catalog.IntegrationEvents.Events
{
    public class ProductPriceChangedIntegrationEvent
    {
        public int ProductId { get; }
        public decimal OldPrice { get; }
        public decimal NewPrice { get; }
        public DateTime OccurredOn { get; }

        public ProductPriceChangedIntegrationEvent(int productId, decimal oldPrice, decimal newPrice)
        {
            ProductId = productId;
            OldPrice = oldPrice;
            NewPrice = newPrice;
            OccurredOn = DateTime.UtcNow;
        }
    }
}
