using System.Text.Json;
using Basket.Model;
using StackExchange.Redis;

namespace Basket.Repository
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IConnectionMultiplexer _mux;
        private readonly IDatabase _db;
        private readonly ILogger<BasketRepository> _logger;

        private const string KeyPrefix = "basket:";                // tránh đụng key khác
        private static string Key(string id) => $"{KeyPrefix}{id}";
        private static readonly TimeSpan Ttl = TimeSpan.FromDays(14);

        private static readonly JsonSerializerOptions JsonOpt = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        public BasketRepository(IConnectionMultiplexer mux, ILogger<BasketRepository> logger)
        {
            _mux = mux;
            _db = mux.GetDatabase();
            _logger = logger;
        }

        public async Task<CustomerBasket> AddOrUpdateItemAsync(CustomerBasket basket)
        {
            if(basket is null) throw new ArgumentNullException(nameof(basket));
            if(string.IsNullOrWhiteSpace(basket.BuyerId))
                throw new ArgumentException("Basket Id is null or empty", nameof(basket));
            var key = Key(basket.BuyerId);
            var raw = await _db.StringGetAsync(key);
            CustomerBasket customerBasket = null;
            if(raw.IsNullOrEmpty)
            {
                // Thêm mới
                var json = JsonSerializer.Serialize(basket, JsonOpt);
                customerBasket = new CustomerBasket { BuyerId = basket.BuyerId, Items = basket.Items };

            }
            else
            {
                customerBasket = JsonSerializer.Deserialize<CustomerBasket>(raw!, JsonOpt)
                    ?? throw new InvalidOperationException($"Deserialize basket failed for {key}");
            }
            var items = basket.Items ??= new List<BasketItem>();
            foreach (var item in items)
            {
                var existingItem = customerBasket.Items.FirstOrDefault(i => i.ProductId == item.ProductId);
                if (existingItem != null)
                {
                    // Cộng dồn số lượng
                    existingItem.Quantity += item.Quantity;
                    existingItem.UnitPrice = item.UnitPrice; // cập nhật giá mới
                    existingItem.ProductName = item.ProductName; // cập nhật tên mới
                }
                else
                {
                    // Thêm mới
                    customerBasket.Items.Add(item);
                }
            }
            await _db.StringSetAsync(key, JsonSerializer.Serialize(customerBasket, JsonOpt), Ttl);
            return customerBasket;
        }

        public async Task<bool> DeleteBasketAsync(string id)
        {
            var removed = await _db.KeyDeleteAsync(Key(id));
            if (!removed) _logger.LogInformation("DeleteBasket: key {Key} not found", Key(id));
            return removed;
        }

        public async Task<CustomerBasket?> GetBasketAsync(string customerId)
        {
            // Cách đơn giản & rõ ràng
            var data = await _db.StringGetAsync(Key(customerId));
            if (data.IsNullOrEmpty) return null;

            try
            {
                return JsonSerializer.Deserialize<CustomerBasket>(data!, JsonOpt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Deserialize basket failed for {Key}", Key(customerId));
                return null;
            }

            /* Nếu muốn tối ưu copy bộ nhớ, dùng lease:
            using var lease = await _db.StringGetLeaseAsync(Key(customerId));
            if (lease is null || lease.Length == 0) return null;
            return JsonSerializer.Deserialize<CustomerBasket>(lease.Span, JsonOpt);
            */
        }

        public async Task<int> UpdatePriceForProductAsync(int productId, decimal newPrice, CancellationToken ct = default)
        {
            // lấy 1 server để SCAN (dev/đơn nút ok; cluster cần lặp qua tất cả endpoints)
            var endpoints = _mux.GetEndPoints();
            var endpoint = _mux.GetEndPoints().First();
            var server = _mux.GetServer(endpoint);

            int touched = 0;

            // SCAN theo prefix để tránh quét toàn bộ DB
            foreach (var key in server.Keys(pattern: $"{KeyPrefix}*", pageSize: 512))
            {
                ct.ThrowIfCancellationRequested();

                var raw = await _db.StringGetAsync(key);
                if (raw.IsNullOrEmpty) continue;

                var basket = JsonSerializer.Deserialize<CustomerBasket>(raw!, JsonOpt);
                if (basket is null || basket.Items is null || basket.Items.Count == 0) continue;

                var changed = false;
                foreach (var it in basket.Items)
                {
                    if (it.ProductId == productId && it.UnitPrice != newPrice)
                    {
                        it.UnitPrice = newPrice;
                        changed = true;
                    }
                }

                if (changed)
                {
                    var json = JsonSerializer.Serialize(basket, JsonOpt);
                    await _db.StringSetAsync(key, json, Ttl); // gia hạn TTL
                    touched++;
                }
            }

            return touched;
        }
    }
}
