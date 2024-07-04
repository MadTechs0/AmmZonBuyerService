using BuyerService.Context;
using BuyerService.Interface;
using BuyerService.Models;
using BuyerService.Service;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BuyerBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly RabbitMQService _rabbitMQService;
        private readonly IOrderService _orderService;

        public OrderController(RabbitMQService rabbitMQService, IOrderService orderService)
        {
            _rabbitMQService = rabbitMQService;
            _orderService = orderService;
        }

        [HttpPost("PostOrder")]
        public async Task<IActionResult> PostOrder(OrderRequest order)
        {
            Guid orderId = await _orderService.PostOrder(order);
            order.Id = orderId;
            _rabbitMQService.PublishOrder(order);
            return Ok();
        }
    }
}
