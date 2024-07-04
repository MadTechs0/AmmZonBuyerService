using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuyerService.Models
{
    public class OrderRequest
    {
        public Guid? Id { get; set; }
        public int OrderValue { get; set; }
        public string Status { get; set; }
        public DateTime OrderDate { get; set; }
        public List<ProductRequest> Products { get; set; } = new List<ProductRequest>();
    }
}
