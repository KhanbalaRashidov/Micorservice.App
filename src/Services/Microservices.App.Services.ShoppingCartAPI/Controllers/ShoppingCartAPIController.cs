using AutoMapper;
using Azure;
using Microservices.App.Services.ShoppingCartAPI.Data;
using Microservices.App.Services.ShoppingCartAPI.Models;
using Microservices.App.Services.ShoppingCartAPI.Models.Dtos;
using Microservices.App.Services.ShoppingCartAPI.Service.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.PortableExecutable;

namespace Microservices.App.Services.ShoppingCartAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class ShoppingCartAPIController : ControllerBase
    {
        private ResponseDto responseDto;
        private IMapper mapper;
        private IProductService productService;
        private readonly AppDbContext dbContext;

        public ShoppingCartAPIController(AppDbContext dbContext, IMapper mapper, IProductService productService)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.responseDto = new ResponseDto();
            this.productService = productService;
        }

        [HttpGet("GetCart/{userId}")]
        public async Task<ResponseDto> GetCart(string userId)
        {
            try
            {
                var cartHeader = await this.dbContext.CartHeaders.FirstOrDefaultAsync(u => u.UserId == userId);
                var cart = new CartDto()
                {
                    CartHeader = this.mapper.Map<CartHeaderDto>(cartHeader)
                };

                cart.CartDetails = mapper.Map<IEnumerable<CartDetailsDto>>(this.dbContext.CartDetails
                    .Where(u => u.CartHeaderId == cart.CartHeader.CartHeaderId));

                var products = await this.productService.GetProductsAsync();

                foreach (var item in cart.CartDetails)
                {
                    item.Product = products.FirstOrDefault(p => p.ProductId == item.ProductId);
                    cart.CartHeader.CartTotal += (item.Count * item.Product.Price);
                }

                responseDto.Result = cart;
            }
            catch (Exception ex)
            {
                responseDto.IsSuccess = false;
                responseDto.Message = ex.Message;

            }

            return responseDto;
        }

        [HttpPost("ApplyCoupon")]
        public async Task<object> ApplyCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                var cartFromDb = this.dbContext.CartHeaders.First(u => u.UserId == cartDto.CartHeader.UserId);
                cartFromDb.CouponCode = cartDto.CartHeader.CouponCode;
            }
            catch (Exception ex)
            {
                responseDto.IsSuccess = false;
                responseDto.Message = ex.Message;
            }
        }

        [HttpPost("CartUpsert")]
        public async Task<ResponseDto> CartUpsert(CartDto cartDto)
        {
            try
            {
                var cartHeaderFromDb = await this.dbContext.CartHeaders.AsNoTracking()
                    .FirstOrDefaultAsync(u => u.UserId == cartDto.CartHeader.UserId);
                if (cartHeaderFromDb == null)
                {
                    //create header and details
                    CartHeader cartHeader = this.mapper.Map<CartHeader>(cartDto.CartHeader);
                    dbContext.CartHeaders.Add(cartHeader);
                    await dbContext.SaveChangesAsync();
                    cartDto.CartDetails.First().CartHeaderId = cartHeader.CartHeaderId;
                    dbContext.CartDetails.Add(mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                    await dbContext.SaveChangesAsync();
                }
                else
                {
                    //if header is not null
                    //check if details has same product
                    var cartDetailsFromDb = await dbContext.CartDetails.AsNoTracking().FirstOrDefaultAsync(
                        u => u.ProductId == cartDto.CartDetails.First().ProductId &&
                        u.CartHeaderId == cartHeaderFromDb.CartHeaderId);
                    if (cartDetailsFromDb == null)
                    {
                        //create cartdetails
                        cartDto.CartDetails.First().CartHeaderId = cartHeaderFromDb.CartHeaderId;
                        dbContext.CartDetails.Add(mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        //update count in cart details
                        cartDto.CartDetails.First().Count += cartDetailsFromDb.Count;
                        cartDto.CartDetails.First().CartHeaderId = cartDetailsFromDb.CartHeaderId;
                        cartDto.CartDetails.First().CartDetailsId = cartDetailsFromDb.CartDetailsId;
                        dbContext.CartDetails.Update(mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                        await dbContext.SaveChangesAsync();
                    }
                }
                responseDto.Result = cartDto;
            }
            catch (Exception ex)
            {
                responseDto.Message = ex.Message.ToString();
                responseDto.IsSuccess = false;
            }
            return responseDto;
        }

        [HttpPost("RemoveCart")]
        public async Task<ResponseDto> RemoveCart([FromBody] int cartDetailsId)
        {
            try
            {
                var cartDetails = this.dbContext.CartDetails.First(u => u.CartDetailsId == cartDetailsId);

                var totalCountOfCartItem = this.dbContext.CartDetails.Where(u => u.CartHeaderId == cartDetails.CartHeaderId).Count();
                this.dbContext.CartDetails.Remove(cartDetails);

                if (totalCountOfCartItem == 1)
                {
                    var cartHeaderToRemove = await this.dbContext.CartHeaders.FirstOrDefaultAsync(
                        u => u.CartHeaderId == cartDetails.CartHeaderId);

                    this.dbContext.CartHeaders.Remove(cartHeaderToRemove);
                }

                await this.dbContext.SaveChangesAsync();

                responseDto.Result = true;
            }
            catch (Exception ex)
            {
                responseDto.Message = ex.Message.ToString();
                responseDto.IsSuccess = false;
            }
            return responseDto;
        }
    }
}
