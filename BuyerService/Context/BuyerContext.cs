using BuyerService.EntityModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuyerService.Context
{
    public class BuyerContext: DbContext
    {
        public BuyerContext(DbContextOptions<BuyerContext> options) : base(options) { }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderProductMapping> OrderProductMappings { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartProductMapping> CartProductMappings { get; set; }
        public DbSet<WishList> WishLists { get; set; }
        public DbSet<WishListProductMapping> WishListsProductMappings { get;set; }
    }
}
