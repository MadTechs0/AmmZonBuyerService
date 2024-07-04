using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuyerService.EntityModels
{
    public class Cart
    {
        public Guid Id { get; set; }
        public decimal CartValue { get; set; }
        public int Items { get; set; }
    }
}
