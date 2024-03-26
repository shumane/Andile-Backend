using System.ComponentModel.DataAnnotations;

namespace Andile_BE.Models
{
    public class Product
    {
        public string? Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Description { get; set; }
        [Required]
        public decimal Price { get; set; }
    }
}
