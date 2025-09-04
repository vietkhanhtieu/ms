using System.ComponentModel.DataAnnotations;

namespace Basket.Model
{
    public class Basket
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        public string ProductName { get; set; }
    }
}
