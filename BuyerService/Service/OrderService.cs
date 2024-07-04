using AutoMapper;
using BuyerService.Context;
using BuyerService.EntityModels;
using BuyerService.Interface;
using BuyerService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuyerService.Service
{
    public class OrderService : IOrderService
    {
        private readonly BuyerContext _dbContext;
        private readonly IMapper _mapper;

        public OrderService(BuyerContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public async Task<Guid> PostOrder(OrderRequest newOrder)
        {
            try
            {
                Order order = _mapper.Map<Order>(newOrder);
                await _dbContext.Orders.AddAsync(order);
                await _dbContext.SaveChangesAsync();

                List<OrderProductMapping> orderProductMappings = new List<OrderProductMapping>();
                for (int i= 0; i<newOrder.Products.Count; i++)
                {
                    orderProductMappings.Add(new OrderProductMapping
                    {
                        OrderId = order.Id,
                        ProductId = newOrder.Products[i].id,
                        Quantity = newOrder.Products[i].Quantity,
                    }); 
                }
                await _dbContext.OrderProductMappings.AddRangeAsync(orderProductMappings);
                await _dbContext.SaveChangesAsync();
                return order.Id;
            }
            catch (Exception ex)
            {
                return Guid.Empty;
            }
        }

        public async Task<bool> UpdateStatus(OrderStatus status)
        {
            try
            {
                var order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.Id == status.OrderId);

                if (order == null)
                {
                    return false;
                }
                if(status.Status)
                {
                    Cart? userCart = await _dbContext.Carts.FirstOrDefaultAsync();
                    List<OrderProductMapping> orderItems = await _dbContext.OrderProductMappings.Where(o => o.OrderId == status.OrderId).ToListAsync();
                    IEnumerable<CartProductMapping>? cartItems = await _dbContext.CartProductMappings.Where(c => c.CartId == userCart.Id).ToListAsync();

                    userCart.CartValue -= order.OrderValue;
                    foreach (var item in orderItems)
                    {
                        userCart.Items -= item.Quantity;
                    }
                    _dbContext.CartProductMappings.RemoveRange(cartItems);

                    await _dbContext.SaveChangesAsync();
                }

                order.Status = status.Status ? "Confirmed" : "Cancelled";

                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
    }
}
