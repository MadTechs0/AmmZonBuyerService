using AutoMapper;
using BuyerService.Context;
using BuyerService.EntityModels;
using BuyerService.Interface;
using BuyerService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BuyerService.Service
{
    public class UserService : IUserService
    {
        private readonly BuyerContext _dbContext;
        private readonly IMapper _mapper; 
        private readonly HttpClient _httpClient;
        private readonly string _productServiceUrl = "https://localhost:7251/api/Product/GetProductDetails";
        public UserService(BuyerContext dbContext, IMapper mapper, HttpClient httpClient) 
        { 
            _dbContext = dbContext;
            _mapper = mapper;
            _httpClient = httpClient;
        }

        public async Task<bool> AddToCart(Product product)
        {
            try
            {
                Cart? userCart = await _dbContext.Carts.FirstOrDefaultAsync();

                if(userCart == null)
                {
                    var newCart = new Cart
                    {
                        CartValue = product.Price,
                        Items = 1,
                    };
                    await _dbContext.Carts.AddAsync(newCart);
                    await _dbContext.SaveChangesAsync();
                    await _dbContext.CartProductMappings.AddAsync(new CartProductMapping { ProductId = product.Id, CartId = newCart.Id, Quantity = 1 });
                    await _dbContext.SaveChangesAsync();
                    return true;
                }
                else
                {
                    List<Guid> cartItems = await _dbContext.CartProductMappings
                        .Select(c => c.ProductId)
                        .ToListAsync();

                    if(cartItems.Contains(product.Id))
                    {
                        return false;
                    }
                    else
                    {
                        userCart.CartValue += product.Price;
                        userCart.Items += 1;
                        await _dbContext.CartProductMappings.AddAsync(new CartProductMapping { ProductId = product.Id, CartId = userCart.Id, Quantity = 1 });
                        await _dbContext.SaveChangesAsync();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> RemoveItem(Product product)
        {
            try
            {
                Cart? userCart = await _dbContext.Carts.FirstOrDefaultAsync();

                if (userCart == null)
                {
                    return false;
                }
                var productMapping = await _dbContext.CartProductMappings
                .FirstOrDefaultAsync(c => c.CartId == userCart.Id && c.ProductId == product.Id);

                if (productMapping == null)
                {
                    return false;
                }
                userCart.Items -= productMapping.Quantity; 
                userCart.CartValue -= product.Price;
                _dbContext.CartProductMappings.Remove(productMapping);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<List<OrderRequest>> MyOrders()
        {
            try
            {
                List<OrderRequest> myOrders =  _mapper.Map<List<OrderRequest>>(await _dbContext.Orders.ToListAsync());
                foreach(OrderRequest order in myOrders)
                {
                    /*order.Products[0].id = await _dbContext.OrderProductMappings.Where(map => map.OrderId == order.Id).Select(map => map.ProductId).ToListAsync();
                    order.Products.Quantity = await _dbContext.OrderProductMappings.Select(c => c.Quantity).ToListAsync();*/
                }
                return myOrders;
            }
            catch(Exception ex)
            {
                return new List<OrderRequest>();
            }
        }
        
        public async Task<CartRequest> CartDetails()
        {
            try
            {
                var cartRequest = await _dbContext.Carts
                .Select(cart => new CartRequest
                {
                    CartId = cart.Id,
                    CartValue = cart.CartValue,
                    Items = cart.Items
                })
                .FirstOrDefaultAsync();

                List<Guid> productIds = await _dbContext.CartProductMappings
                        .Where(mapping => mapping.CartId == cartRequest.CartId)
                        .Select(mapping => mapping.ProductId)
                        .ToListAsync();
                var productDetails = await FetchProductDetailsAsync(productIds);
                cartRequest.Products = productDetails;

                foreach(var product in productDetails) 
                {
                    product.Quantity = await _dbContext.CartProductMappings
                        .Where(c => c.CartId == cartRequest.CartId && c.ProductId == product.id)
                        .Select(c => c.Quantity)
                        .FirstOrDefaultAsync();
                }
                return cartRequest;
            }
            catch( Exception ex )
            {
                return new CartRequest();
            }
        }
        private async Task<List<ProductRequest>> FetchProductDetailsAsync(List<Guid> productIds)
        {
            var query = string.Join("&", productIds.Select(id => $"productIds={id}"));
            var requestUrl = $"{_productServiceUrl}?{query}";

            var response = await _httpClient.GetAsync(requestUrl);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            var productDetails = JsonSerializer.Deserialize<List<ProductRequest>>(responseString);


            return productDetails;
        }
        public async Task<bool> ChangeQuantity(QuantityRequest actionItem)
        {
            try
            {
                if(actionItem.Action == true)
                {
                    Cart? userCart = await _dbContext.Carts.FirstOrDefaultAsync();

                    if (userCart == null)
                    {
                        return false;
                    }
                    var productMapping = await _dbContext.CartProductMappings
                    .FirstOrDefaultAsync(c => c.CartId == userCart.Id && c.ProductId == actionItem.Product.Id);

                    if (productMapping == null)
                    {
                        return false;
                    }
                    userCart.Items += 1;
                    productMapping.Quantity += 1;
                    userCart.CartValue += actionItem.Product.Price;
                    await _dbContext.SaveChangesAsync();
                    return true;
                }
                else
                {

                    Cart? userCart = await _dbContext.Carts.FirstOrDefaultAsync();

                    if (userCart == null)
                    {
                        return false;
                    }
                    var productMapping = await _dbContext.CartProductMappings
                    .FirstOrDefaultAsync(c => c.CartId == userCart.Id && c.ProductId == actionItem.Product.Id);

                    if (productMapping == null)
                    {
                        return false;
                    }
                    userCart.Items -= 1;
                    productMapping.Quantity -= 1;
                    userCart.CartValue -= actionItem.Product.Price;
                    await _dbContext.SaveChangesAsync();
                    return true;
                }
            }
            catch ( Exception ex )
            {
                return false;
            }
        }
        public async Task<CartRequest> WishListDetails()
        {
            try
            {
                var wishRequest = await _dbContext.WishLists
                .Select(wish => new CartRequest
                {
                    CartId = wish.Id,
                    Items = wish.Items
                })
                .FirstOrDefaultAsync();

                List<Guid> productIds = await _dbContext.WishListsProductMappings
                        .Where(mapping => mapping.WishListId == wishRequest.CartId)
                        .Select(mapping => mapping.ProductId)
                        .ToListAsync();

                var productDetails = await FetchProductDetailsAsync(productIds);
                wishRequest.Products = productDetails;

                foreach (var product in productDetails)
                {
                    product.Quantity = 1;
                }
                return wishRequest;
            }
            catch (Exception ex)
            {
                return new CartRequest();
            }
        }
        public async Task<bool> AddToWish(Product product)
        {
            try
            {
                WishList? userWish = await _dbContext.WishLists.FirstOrDefaultAsync();

                if (userWish == null)
                {
                    var newWish = new WishList
                    {
                        Items = 1,
                    };
                    await _dbContext.WishLists.AddAsync(newWish);
                    await _dbContext.SaveChangesAsync();
                    await _dbContext.WishListsProductMappings.AddAsync(new WishListProductMapping { ProductId = product.Id, WishListId = newWish.Id, Quantity = 1 });
                    await _dbContext.SaveChangesAsync();
                    return true;
                }
                else
                {
                    List<Guid> wishItems = await _dbContext.WishListsProductMappings
                        .Select(c => c.ProductId)
                        .ToListAsync();

                    if (wishItems.Contains(product.Id))
                    {
                        return false;
                    }
                    else
                    {
                        userWish.Items += 1;
                        await _dbContext.WishListsProductMappings.AddAsync(new WishListProductMapping { ProductId = product.Id, WishListId = userWish.Id, Quantity = 1 });
                        await _dbContext.SaveChangesAsync();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> RemoveWish(Product product)
        {
            try
            {
                WishList? userWish = await _dbContext.WishLists.FirstOrDefaultAsync();

                if (userWish == null)
                {
                    return false;
                }
                var productMapping = await _dbContext.WishListsProductMappings
                .FirstOrDefaultAsync(c => c.WishListId == userWish.Id && c.ProductId == product.Id);

                if (productMapping == null)
                {
                    return false;
                }
                userWish.Items -= productMapping.Quantity;
                _dbContext.WishListsProductMappings.Remove(productMapping);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> AddWishToCart(Product product)
        {
            try
            {
                Cart userCart = await _dbContext.Carts.FirstOrDefaultAsync();
                if (userCart == null)
                {
                    var newCart = new Cart
                    {
                        CartValue = product.Price,
                        Items = product.Quantity,
                    };
                    await _dbContext.Carts.AddAsync(newCart);
                    await _dbContext.SaveChangesAsync();
                    await _dbContext.CartProductMappings.AddAsync(new CartProductMapping { ProductId = product.Id, CartId = newCart.Id, Quantity = product.Quantity });
                    await RemoveWish(product);
                    await _dbContext.SaveChangesAsync();
                    return true;
                }
                else
                {
                    CartProductMapping? productMapping = await _dbContext.CartProductMappings.Where(map => map.CartId == userCart.Id && map.ProductId == product.Id).FirstOrDefaultAsync();
                    if(productMapping == null)
                    {
                        await _dbContext.CartProductMappings.AddAsync(new CartProductMapping
                        {
                            ProductId = product.Id,
                            Quantity = product.Quantity,
                        });
                        userCart.Items += product.Quantity;
                        userCart.CartValue += product.Price;
                        await RemoveWish(product);
                        await _dbContext.SaveChangesAsync();

                        return true;
                    }
                    else
                    {
                        productMapping.Quantity += product.Quantity;
                        userCart.CartValue += product.Price;
                        userCart.Items += product.Quantity;
                        await RemoveWish(product);
                        await _dbContext.SaveChangesAsync();

                        return true;
                    }

                }
            }
            catch(Exception ex)
            {
                return false;
            }
        }
    }
}
