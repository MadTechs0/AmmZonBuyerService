using BuyerService.EntityModels;
using BuyerService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuyerService.Interface
{
    public interface IOrderService
    {
        /*Task<bool> ProcessOrder(Order newOrder);*/
        Task<Guid> PostOrder(OrderRequest newOrder);
        Task<bool> UpdateStatus(OrderStatus status);
    }
}
