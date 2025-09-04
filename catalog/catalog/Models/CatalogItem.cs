using System.ComponentModel.DataAnnotations;

namespace catalog.Models
{
    public class CatalogItem
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }
        public string PictureFileName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
