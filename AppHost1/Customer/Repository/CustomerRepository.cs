using Customer.Contractor;
using Customer.Dtos;
using Customer.Models;

namespace Customer.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly CustomerContext _context;

        public CustomerRepository(CustomerContext context)
        {
            _context = context;
        }
        async Task<CustomerEntity> Create(CustomerDto customerDto)
        {
            return new NotImplementedException();
        }
        async Task<CustomerEntity> Update(CustomerDto customerDto)
        {
            return new NotImplementedException();
        }
        async Task<CustomerEntity> Delete(Guid customerId)
        {

        }
        async Task<CustomerEntity> Login(CustomerDto customerDto)
        {
            return NotImplementedException();
        }

    }
}
