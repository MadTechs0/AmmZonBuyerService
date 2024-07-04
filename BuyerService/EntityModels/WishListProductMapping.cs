using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuyerService.EntityModels
{
    public class WishListProductMapping
    {
        public Guid Id { get; set; }
        public Guid WishListId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public WishList WishList { get; set; }
    }
}
