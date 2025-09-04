using catalog.Models;
using Microsoft.EntityFrameworkCore;

namespace catalog.Repository
{
    public class CatalogRepository
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
    }
}
