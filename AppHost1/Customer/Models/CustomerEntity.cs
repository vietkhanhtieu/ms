using System.ComponentModel.DataAnnotations;

namespace Customer.Models
{
    public class CustomerEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Disctrict {  get; set; }
        public string Wards { get; set; }
        public string Avartar { get; set; }
    }
}
