using AutoMapper;
using BuyerService.EntityModels;
using BuyerService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuyerService.Mapper
{
    public class MappingProfile: Profile
    {
        public MappingProfile() 
        {
            CreateMap<OrderRequest, Order>();
            CreateMap<Order, OrderRequest>();
        }

    }
}
