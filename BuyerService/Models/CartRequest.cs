using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuyerService.Models
{
    public class CartRequest
    {
        public Guid CartId { get; set; }
        public decimal CartValue { get; set; }
        public int Items { get; set; }
        public List<ProductRequest> Products { get; set; }
    }
}
