namespace Andile_BE.Controllers
{
    using Andile_BE.Interfaces;
    using Andile_BE.Models;
    using Microsoft.AspNetCore.Mvc;
    using Serilog;

    [Route("[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService) => _customerService = customerService;

        [HttpPost]
        public IActionResult CreateCustomer([FromBody] Customer customer)
        {
            try
            {
                _customerService.CreateCustomer(customer);
                Log.Information($"{customer.Name} was created successfully");

                return Ok();
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurred while creating {customer.Name} with id({customer.Id}) => {ex.Message}");
                throw;
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCustomer(string id)
        {
            try
            {
                _customerService.DeleteCustomer(id);
                Log.Information($"Customer with id: {id} was deleted successfully");

                return Ok();
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurred while deleting customer with id({id}) => {ex.Message}");
                throw;
            }
        }

        [HttpGet("all-customers")]
        public ActionResult<IEnumerable<Customer>> GetAllCustomers()
        {
            try
            {
                var customers = _customerService.GetAllCustomers();
                Log.Information($"Customers retrieved successfully");
                return Ok(customers);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while retrieving all orders");
                throw;
            }
        }
    }
}
