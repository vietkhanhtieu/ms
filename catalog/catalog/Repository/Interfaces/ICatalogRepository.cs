using catalog.Models;

namespace catalog.Repository.Interfaces
{
    public interface ICatalogRepository
    {
        Task<List<CatalogItem>> GetCatalogItemsAsync();
        Task<CatalogItem?> GetCatalogItemByIdAsync(int id);
        Task AddCatalogItemAsync(CatalogItem item);
        Task UpdateCatalogItemAsync(CatalogItem item);
        Task DeleteCatalogItemAsync(int id);
    }
}
