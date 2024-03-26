namespace Andile_BE.Interfaces
{
    using Andile_BE.Models;

    public interface IProductService
    {
        Product AddProduct(Product product);
        void RemoveProducts(string[] ids);
        List<Product> GetProductsByIds(string[] ids);
        Product GetProductById(string id);
        Product UpdateProduct(string id, Product updatedProduct);
        List<Product> GetAllProducts(int page, int pageSize);
    }
}
