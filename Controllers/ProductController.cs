using Andile_BE.Interfaces;
using Andile_BE.Models;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Andile_BE.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService) => _productService = productService;

        [HttpPost]
        public ActionResult<Product> CreateProduct([FromBody] Product product)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var newProduct = _productService.AddProduct(product);

                Log.Information($"CreateProduct => {newProduct.Name} was created successfully");
                return CreatedAtAction(nameof(GetProductById),
                    new { id = newProduct.Id },
                    newProduct);
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurred while creating {product.Name} => {ex.Message}");
                throw;
            }
        }

        [HttpPost("remove-products")]
        public IActionResult RemoveProducts([FromBody] string[] ids)
        {
            try
            {
                _productService.RemoveProducts(ids);

                Log.Information($"Products with ids : {string.Join(", ", ids)} were removed successfully");
                return Ok();
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurred removing products => {ex.Message}");
                throw;
            }
        }

        [HttpGet("{id}")]
        public ActionResult<Product> GetProductById(string id)
        {
            try
            {
                var product = _productService.GetProductById(id);
                if (product is null)
                    return NotFound();

                Log.Information($"Product retrieved successfully");
                return Ok(product);
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurred while getting the product with ID: {id} => {ex.Message}");
                throw;
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateProduct(string id, [FromBody] Product updatedProduct)
        {
            try
            {
                var product = _productService.UpdateProduct(id, updatedProduct);
                if (product is null)
                    return NotFound();

                Log.Information($"{product.Name} updated successfully");
                return Ok(product);
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurred while updating {updatedProduct.Name} => {ex.Message}");
                throw;
            }
        }

        [HttpGet("all")]
        public ActionResult<List<Product>> GetAllProducts(int page = 1, int pageSize = 4)
        {
            try
            {
                var products = _productService.GetAllProducts(page, pageSize);
                return Ok(products);
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurred while retrieving all products => {ex.Message}");
                throw;
            }
        }
    }
}
