using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuyerService.Models
{
    public class QuantityRequest
    {
        public bool Action { get; set; }
        public Product Product { get; set; }
    }
}
