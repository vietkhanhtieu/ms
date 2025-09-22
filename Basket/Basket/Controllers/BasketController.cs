using Basket.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Basket.Controllers
{
    [Route("api/[controller]")]
    public class BasketController  : ControllerBase
    {
        private readonly ILogger<BasketController> _logger;
        private readonly IBasketRepository _basketRepository;

        public BasketController(ILogger<BasketController> logger, IBasketRepository basketRepository)
        {
            _basketRepository = basketRepository ?? throw new ArgumentNullException(nameof(basketRepository));
            _logger = logger;
        }

        [HttpPost("CreateItem")]
        public async Task<IActionResult> CreateItem([FromBody] Model.CustomerBasket basket)
        {
            var result = await _basketRepository.AddOrUpdateItemAsync(basket);
            return Ok(result);
        }

        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetBasketById(string customerId)
        {
            var result = await _basketRepository.GetBasketAsync(customerId);
            if (result == null)
                return NotFound();
            return Ok(result);
        }
    }
}
