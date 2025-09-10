using catalog.Models;
using catalog.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace catalog.Repository.Implementations
{
    public class CatalogRepository : ICatalogRepository
    {
        private readonly CatalogContext _context;

        public CatalogRepository(CatalogContext context)
        {
            _context = context;
        }

        public async Task<List<CatalogItem>> GetCatalogItemsAsync()
        {
            return await _context.CatalogItems.AsNoTracking().ToListAsync();
        }

        public async Task<CatalogItem?> GetCatalogItemByIdAsync(int id)
        {
            return await _context.CatalogItems.AsNoTracking().FirstOrDefaultAsync(ci => ci.Id == id);
        }
        public async Task AddCatalogItemAsync(CatalogItem item)
        {
            _context.CatalogItems.Add(item);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateCatalogItemAsync(CatalogItem item)
        {
            _context.CatalogItems.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCatalogItemAsync(int id)
        {
            var item = await _context.CatalogItems.FindAsync(id);
            if (item != null)
            {
                _context.CatalogItems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }
    }
}
