
namespace Andile_BE.Services
{
    using Andile_BE.Interfaces;
    using Andile_BE.Models;
    using Microsoft.Data.SqlClient;
    using System.Collections.Generic;

    public class CustomerService : ICustomerService
    {
        private readonly string _connectionString;

        public CustomerService(IConfiguration configuration) => _connectionString = configuration.GetConnectionString("DefaultConnection");

        public Customer CreateCustomer(Customer customer)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sql = "INSERT INTO Customer (Id, Name, Email) VALUES (@Id, @Name, @Email)";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Id", customer.Id);
                command.Parameters.AddWithValue("@Name", customer.Name);
                command.Parameters.AddWithValue("@Email", customer.Email);

                connection.Open(); // Connection will be opened here
                command.ExecuteNonQuery();
            }

            // Connection will be closed here
            return customer;
        }

        public void DeleteCustomer(string id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sql = "DELETE FROM Customer WHERE Id = @Id";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Id", id);

                connection.Open(); // Connection will be opened here
                command.ExecuteNonQuery();
            }
        }

        public List<Customer> GetAllCustomers()
        {
            List<Customer> customers = new List<Customer>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sql = "SELECT Id, Name, Email FROM Customer";
                SqlCommand command = new SqlCommand(sql, connection);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Customer order = new Customer
                    {
                        Id = reader.GetString(0),
                        Name = reader.GetString(1),
                        Email = reader.GetString(2)
                    };

                    customers.Add(order);
                }
                reader.Close();
            }
            return customers;
        }

    }
}
