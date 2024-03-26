namespace Andile_BE.Services
{
    using Andile_BE.Interfaces;
    using Andile_BE.Models;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Collections.Generic;
    public class OrderService : IOrderService
    {
        private readonly string _connectionString;

        public OrderService(IConfiguration configuration) => _connectionString = configuration.GetConnectionString("DefaultConnection");

        public Order CreateOrder(Order order)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "INSERT INTO Orders (Id, HasPaid, CustomerId, Total) VALUES (@Id, @HasPaid, @CustomerId, @Total)";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Id", order.Id);
                command.Parameters.AddWithValue("@HasPaid", order.HasPaid);
                command.Parameters.AddWithValue("@CustomerId", order.CustomerId);
                command.Parameters.AddWithValue("@Total", order.Total);

                connection.Open();
                command.ExecuteNonQuery();
            }

            return order;
        }

        public List<Order> GetOrdersByIds(string[] ids, string customerId = null)
        {
            List<Order> orders = new List<Order>();

            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "SELECT Id, HasPaid, CustomerId, Total FROM Orders WHERE Id IN ({0})";
                string parameterNames = string.Join(",", Array.ConvertAll(ids, _ => "@Id" + _));
                SqlCommand command = new SqlCommand(string.Format(sql, parameterNames), connection);

                for (int i = 0; i < ids.Length; i++)
                {
                    command.Parameters.AddWithValue("@Id" + ids[i], ids[i]);
                }

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Order order = new Order
                    {
                        Id = reader.GetString(0),
                        HasPaid = reader.GetBoolean(1),
                        CustomerId = reader.GetString(2),
                        Total = reader.GetDecimal(3)
                    };

                    orders.Add(order);
                }

                reader.Close();
            }

            if (!string.IsNullOrEmpty(customerId))
                orders = orders.FindAll(o => o.CustomerId == customerId);

            return orders;
        }

        public Order GetOrderById(string id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sql = "SELECT Id, HasPaid, CustomerId, Total FROM Orders WHERE Id = @Id";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Id", id);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    Order order = new Order
                    {
                        Id = reader.GetString(0),
                        HasPaid = reader.GetBoolean(1),
                        CustomerId = reader.GetString(2),
                        Total = reader.GetDecimal(3)
                    };
                    return order;
                }
                return null;
            }
        }

        public List<Order> GetAllOrders()
        {
            List<Order> orders = new List<Order>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sql = "SELECT Id, HasPaid, CustomerId, Total FROM Orders";
                SqlCommand command = new SqlCommand(sql, connection);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Order order = new Order
                    {
                        Id = reader.GetString(0),
                        HasPaid = reader.GetBoolean(1),
                        CustomerId = reader.GetString(2),
                        Total = reader.GetDecimal(3)
                    };
                    orders.Add(order);
                }
                reader.Close();
            }

            return orders;
        }


        public Order UpdateOrder(string id, Order updatedOrder)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sql = "UPDATE Orders SET HasPaid = @HasPaid, CustomerId = @CustomerId, Total = @Total WHERE Id = @Id";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Id", id);
                command.Parameters.AddWithValue("@HasPaid", updatedOrder.HasPaid);
                command.Parameters.AddWithValue("@CustomerId", updatedOrder.CustomerId);
                command.Parameters.AddWithValue("@Total", updatedOrder.Total);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    updatedOrder.Id = id; // Update the ID in the updatedOrder object
                    return updatedOrder;
                }

                return null; // Return null if the order with the specified ID does not exist
            }
        }
    }
}
