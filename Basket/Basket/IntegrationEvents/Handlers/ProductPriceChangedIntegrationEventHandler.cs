using Basket.Infractructure.EvenBus;
using Basket.IntegrationEvents.Events;

namespace Basket.IntegrationEvents.Handlers
{
    public class ProductPriceChangedIntegrationEventHandler : IIntegrationEventHandler<ProductPriceChangedIntegrationEvent>
    {
        public ProductPriceChangedIntegrationEventHandler() { }

        public async Task Handle(ProductPriceChangedIntegrationEvent entity) 
        {
            // Logic to handle the product price change event
            // For example, update the basket items with the new price
            // This is a placeholder for actual implementation
            Console.WriteLine($"Product Price Changed: ProductId={entity.ProductId}, OldPrice={entity.OldPrice}, NewPrice={entity.NewPrice}, OccurredOn={entity.OccurredOn}");

            await Task.CompletedTask; // Simulating async operation
        }
    }
}
