namespace Andile_BE.Controllers
{
    using Andile_BE.Interfaces;
    using Andile_BE.Models;
    using Microsoft.AspNetCore.Mvc;
    using Serilog;

    [Route("[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService) => _orderService = orderService;

        [HttpPost]
        public ActionResult<Order> CreateOrder([FromBody] Order order)
        {
            try
            {
                var newOrder = _orderService.CreateOrder(order);
                Log.Information($"Order with id :{order.Id} was created successfully");

                return CreatedAtAction(nameof(GetOrderById), new { id = newOrder.Id }, newOrder);
            }
            catch (Exception ex)
            {
                Log.Error($"Error occurred while creating order {order}: {ex.Message}");
                throw;
            }
        }

        [HttpGet]
        public ActionResult<IEnumerable<Order>> GetOrdersByIds([FromBody] string[] ids, string customerId = null)
        {
            try
            {
                var orders = _orderService.GetOrdersByIds(ids, customerId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while retrieving orders");
                throw;
            }
        }

        [HttpGet("{id}")]
        public ActionResult<Order> GetOrderById(string id)
        {
            try
            {
                var order = _orderService.GetOrderById(id);

                if (order is null)
                    return NotFound();
                return Ok(order);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while retrieving order by ID");
                throw;
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateOrder(string id, [FromBody] Order updatedOrder)
        {
            try
            {
                var order = _orderService.UpdateOrder(id, updatedOrder);
                if (order is null)
                    return NotFound();
                return Ok(order);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while updating order {0}", updatedOrder.Id);
                throw;
            }
        }

        [HttpGet("all-orders")]
        public ActionResult<IEnumerable<Order>> GetAllOrders()
        {
            try
            {
                var orders = _orderService.GetAllOrders();
                Log.Information($"Orders retrieved successfully");

                return Ok(orders);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while retrieving all orders");
                throw;
            }
        }

    }
}
