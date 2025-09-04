using catalog.Infrastructure.EventBus;
using catalog.IntegrationEvents.Events;
using catalog.Repository;
using Microsoft.AspNetCore.Mvc;

namespace catalog.Controllers
{
    [Route("api/[controller]")]
    public class CatalogController : ControllerBase
    {
        private readonly CatalogRepository _repository;
        public CatalogController(CatalogRepository catalogRepository)
        {
            _repository = catalogRepository ?? throw new ArgumentNullException(nameof(catalogRepository));

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
            var items = await _repository.GetCatalogItemsAsync();
            var item = items.FirstOrDefault(i => i.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            var oldPrice = item.Price;
            item.Price = newPrice;

            // Here you would typically save the changes to the database
            // await _repository.SaveChangesAsync();

            // Publish the integration event
            var integrationEvent = new ProductPriceChangedIntegrationEvent(item.Id, oldPrice, newPrice);
            var eventBus = new EventBusRabbitMQ("ProductPriceChangedIntegrationEvent");
            eventBus.PublishAsync(integrationEvent);

            return Ok(item);
        }
    }
}
