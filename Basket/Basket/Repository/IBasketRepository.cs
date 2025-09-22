using Basket.Model;

namespace Basket.Repository
{
    public interface IBasketRepository
    {
        Task<CustomerBasket> GetBasketAsync(string customerId);
        Task<int> UpdatePriceForProductAsync(int productId, decimal newPrice, CancellationToken ct = default);
        Task<bool> DeleteBasketAsync(string id);
        Task<CustomerBasket> AddOrUpdateItemAsync(CustomerBasket basket);
    }
}
