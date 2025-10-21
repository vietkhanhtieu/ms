using Customer.Dtos;
using Customer.Models;

namespace Customer.Contractor
{
    public interface ICustomerRepository
    {
        Task<CustomerEntity> Create(CustomerDto customerDto);
        Task<CustomerEntity> Update(CustomerDto customerDto);
        Task<CustomerEntity> Delete(Guid customerId);
        Task<CustomerEntity> Login(CustomerDto customerDto);
    }
}
