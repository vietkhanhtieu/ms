using catalog.IntegrationEvents;
using catalog.IntegrationEvents.Events;
using catalog.Repository.Implementations;
using catalog.Repository.Interfaces;
using EventBusRabbitMQ;
using Microsoft.AspNetCore.Mvc;

namespace catalog.Controllers
{
    [Route("api/[controller]")]
    public class CatalogController : ControllerBase
    {
        private readonly ICatalogRepository _repository;
        private readonly ICatalogIntegrationEventService _catalogIntegrationEventService;

        public CatalogController(ICatalogRepository catalogRepository, ICatalogIntegrationEventService catalogIntegrationEventService)
        {
            _repository = catalogRepository ?? throw new ArgumentNullException(nameof(catalogRepository));
            _catalogIntegrationEventService = catalogIntegrationEventService ?? throw new ArgumentNullException(nameof(catalogIntegrationEventService));
        }

        [HttpGet]
        public async Task<IActionResult> GetCatalogItems()
        {
            var items = await _repository.GetCatalogItemsAsync();
            return Ok(items);
        }

        [HttpPost("update-price/{id}/{newPrice}")]
        public
            async Task<IActionResult> UpdatePrice(int id, decimal newPrice)
        {
            var item = await _repository.GetCatalogItemByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            if (item.Price != newPrice)
            {

                var oldPrice = item.Price;
                item.Price = newPrice;

                // Publish the integration event
                var integrationEvent = new ProductPriceChangedIntegrationEvent(item.Id, oldPrice, newPrice);

                // Use the integration event service to save and publish the event
                await _catalogIntegrationEventService.SaveEventAndCatalogContextChangesAsync(integrationEvent);

                // Publish through the Event Bus and mark the saved event as published
                await _catalogIntegrationEventService.PublishThroughEventBusAsync(integrationEvent);

                //var eventBus = new RabbitMQEventBus("ProductPriceChangedIntegrationEvent");
                //eventBus.PublishAsync(integrationEvent);
            }

            return Ok(item);
        }
    }
}
