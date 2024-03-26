namespace Andile_BE.Interfaces
{
    using Andile_BE.Models;

    public interface IOrderService
    {
        Order CreateOrder(Order order);
        List<Order> GetOrdersByIds(string[] ids, string customerId = null);
        Order GetOrderById(string id);
        Order UpdateOrder(string id, Order updatedOrder);
        List<Order> GetAllOrders();
    }
}
