using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuyerService.Models
{
    public class OrderStatus
    {
        public Guid OrderId { get; set; }
        public bool Status { get; set; }
    }
}
