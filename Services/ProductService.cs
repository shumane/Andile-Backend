namespace Andile_BE.Services
{
    using Andile_BE.Interfaces;
    using Andile_BE.Models;
    using Microsoft.Data.SqlClient;

    public class ProductService : IProductService
    {
        private readonly string _connectionString;
        //I should create a method that will have to initialize a Sql Connection once

        public ProductService(IConfiguration configuration) => _connectionString = configuration.GetConnectionString("DefaultConnection");

        public Product AddProduct(Product product)
        {
            product.Id = Guid.NewGuid().ToString();

            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "INSERT INTO Product (Id, Name, Description, Price) VALUES (@Id, @Name, @Description, @Price)";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Id", product.Id);
                command.Parameters.AddWithValue("@Name", product.Name);
                command.Parameters.AddWithValue("@Description", product.Description ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Price", product.Price);
                connection.Open();
                command.ExecuteNonQuery();
            }

            return product;
        }

        public void RemoveProducts(string[] ids)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = $"DELETE FROM Product WHERE Id IN ({string.Join(",", ids.Select((_, i) => $"@Id{i}"))})";
                SqlCommand command = new SqlCommand(sql, connection);

                connection.Open();
                for (int i = 0; i < ids.Length; i++)
                {
                    command.Parameters.AddWithValue($"@Id{i}", ids[i]);
                }

                command.ExecuteNonQuery();
            }
        }


        public List<Product> GetProductsByIds(string[] ids)
        {
            List<Product> products = new List<Product>();

            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "SELECT Id, Name, Description, Price FROM Product WHERE Id IN ({0})";
                string parameterNames = string.Join(",", Array.ConvertAll(ids, _ => "@Id" + _));
                SqlCommand command = new SqlCommand(string.Format(sql, parameterNames), connection);

                for (int i = 0; i < ids.Length; i++)
                {
                    command.Parameters.AddWithValue("@Id" + ids[i], ids[i]);
                }

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Product product = new Product
                    {
                        Id = reader.GetString(0),
                        Name = reader.GetString(1),
                        Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                        Price = reader.GetDecimal(3)
                    };
                    products.Add(product);
                }
                reader.Close();
            }

            return products;
        }

        public Product GetProductById(string id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "SELECT Id, Name, Description, Price FROM Product WHERE Id = @Id";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    Product product = new Product
                    {
                        Id = reader.GetString(0),
                        Name = reader.GetString(1),
                        Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                        Price = reader.GetDecimal(3)
                    };
                    return product;
                }
                return null;
            }
        }

        public Product UpdateProduct(string id, Product updatedProduct)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "UPDATE Product SET Name = @Name, Description = @Description, Price = @Price WHERE Id = @Id";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Id", id);
                command.Parameters.AddWithValue("@Name", updatedProduct.Name);
                command.Parameters.AddWithValue("@Description", updatedProduct.Description ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Price", updatedProduct.Price);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    updatedProduct.Id = id; // Update the ID in the updatedProduct object
                    return updatedProduct;
                }

                return null; // Return null if the product with the specified ID does not exist
            }
        }

        public List<Product> GetProducts(int page, int pageSize)
        {
            List<Product> products = new List<Product>();

            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "SELECT Id, Name, Description, Price FROM Product ORDER BY Id OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);
                command.Parameters.AddWithValue("@PageSize", pageSize);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Product product = new Product
                    {
                        Id = reader.GetString(0),
                        Name = reader.GetString(1),
                        Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                        Price = reader.GetDecimal(3)
                    };

                    products.Add(product);
                }
                reader.Close();
            }

            return products;
        }

        public List<Product> GetAllProducts(int page, int pageSize)
        {
            List<Product> products = new List<Product>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                int offset = (page - 1) * pageSize;
                string sql = $"SELECT Id, Name, Description, Price FROM Product ORDER BY Id OFFSET {offset} ROWS FETCH NEXT {pageSize} ROWS ONLY";
                SqlCommand command = new SqlCommand(sql, connection);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Product product = new Product
                    {
                        Id = reader.GetString(0),
                        Name = reader.GetString(1),
                        Description = reader.GetString(2),
                        Price = reader.GetDecimal(3)
                    };
                    products.Add(product);
                }
                reader.Close();
            }

            return products;
        }

    }
}
