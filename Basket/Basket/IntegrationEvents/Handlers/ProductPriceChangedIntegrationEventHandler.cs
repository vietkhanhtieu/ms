using Basket.IntegrationEvents.Events;
using Basket.Repository;
using EventBus.Abstractions;
using Microsoft.Extensions.Logging;

namespace Basket.IntegrationEvents.Handlers
{
    public class ProductPriceChangedIntegrationEventHandler : IIntegrationEventHandler<ProductPriceChangedIntegrationEvent>
    {
        private readonly IBasketRepository _basketRepository;
        private readonly ILogger<ProductPriceChangedIntegrationEventHandler> _logger;

        public ProductPriceChangedIntegrationEventHandler(
            IBasketRepository basketRepository,
            ILogger<ProductPriceChangedIntegrationEventHandler> logger)
        {
            _basketRepository = basketRepository;
            _logger = logger;
        }

        public async Task Handle(ProductPriceChangedIntegrationEvent entity) 
        {
            // Logic to handle the product price change event
            // For example, update the basket items with the new price
            // This is a placeholder for actual implementation
            var updated = await _basketRepository.UpdatePriceForProductAsync(entity.ProductId, entity.NewPrice);
            Console.WriteLine($"Product Price Changed: ProductId={entity.ProductId}, OldPrice={entity.OldPrice}, NewPrice={entity.NewPrice}, OccurredOn={entity.OccurredOn}"); 

            await Task.CompletedTask; // Simulating async operation
        }
    }
}
