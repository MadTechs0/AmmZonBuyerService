using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuyerService.Models
{
    public class ProductRequest
    {
        public Guid id { get; set; }
        public string? name { get; set; }
        public decimal? price { get; set; }
        public int Quantity { get; set; }
    }
}
