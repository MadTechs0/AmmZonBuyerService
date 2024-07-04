using BuyerService.Context;
using BuyerService.Interface;
using BuyerService.Models;
using BuyerService.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BuyerBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly RabbitMQService _rabbitMQService;
        private readonly IUserService _userService;

        public UserController(RabbitMQService rabbitMQService, IUserService userService)
        {
            _rabbitMQService = rabbitMQService;
            _userService = userService;
        }

        [HttpPost("AddtoCart")]
        public async Task<IActionResult> AddToCart(Product product)
        {
            return Ok(await _userService.AddToCart(product));
        }

        [HttpPost("RemoveItem")]
        public async Task<IActionResult> RemoveItem(Product product)
        {
            return Ok(await _userService.RemoveItem(product));
        }

        [HttpGet("MyOrders")]
        public async Task<IActionResult> MyOrders()
        {
            return Ok(await _userService.MyOrders());
        }

        [HttpGet("CartDetails")]
        public async Task<IActionResult> CartDetails()
        {
            return Ok(await _userService.CartDetails());
        }

        [HttpPost("ChangeQuantity")]
        public async Task<IActionResult> ChangeQuantity(QuantityRequest actionItem)
        {
            return Ok(await _userService.ChangeQuantity(actionItem));
        }

        [HttpGet("WishListDetails")]
        public async Task<IActionResult> WishListDetails()
        {
            return Ok(await _userService.WishListDetails());
        }

        [HttpPost("AddToWish")]
        public async Task<IActionResult> AddToWish(Product product)
        {
            return Ok(await _userService.AddToWish(product));
        }

        [HttpPost("RemoveWish")]
        public async Task<IActionResult> RemoveWish(Product product)
        {
            return Ok(await _userService.RemoveWish(product));
        }

        [HttpPost("AddWishToCart")]
        public async Task<IActionResult> AddWishToCart(Product product)
        {
            return Ok(await _userService.AddWishToCart(product));
        }
    }
}
