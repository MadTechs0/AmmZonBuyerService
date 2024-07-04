using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuyerService.EntityModels
{
    public class Order
    {
        public Guid Id { get; set; }
        public decimal OrderValue { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
    }
}
