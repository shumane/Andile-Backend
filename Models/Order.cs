namespace Andile_BE.Models
{
    public class Order
    {
        public string? Id { get; set; }
        public bool HasPaid { get; set; }
        public string? CustomerId { get; set; }
        public List<string>? Products { get; set; }
        public decimal Total { get; set; }
    }
}
