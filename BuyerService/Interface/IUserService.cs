using BuyerService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuyerService.Interface
{
    public interface IUserService
    {
        Task<bool> AddToCart(Product product);
        Task<bool> RemoveItem(Product product);
        Task<List<OrderRequest>> MyOrders();
        Task<CartRequest> CartDetails();
        Task<bool> ChangeQuantity(QuantityRequest actionItem);
        Task<CartRequest> WishListDetails();
        Task<bool> AddToWish(Product product);
        Task<bool> RemoveWish(Product product);
        Task<bool> AddWishToCart(Product product);
    }
}
