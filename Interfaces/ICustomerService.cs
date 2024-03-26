namespace Andile_BE.Interfaces
{
    using Andile_BE.Models;

    public interface ICustomerService
    {
        Customer CreateCustomer(Customer customer);
        void DeleteCustomer(string id);
        List<Customer> GetAllCustomers();
    }
}
