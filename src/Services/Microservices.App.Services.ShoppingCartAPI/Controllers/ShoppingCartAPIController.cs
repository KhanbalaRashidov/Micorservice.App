using AutoMapper;
using Azure;
using Microservices.App.Services.ShoppingCartAPI.Data;
using Microservices.App.Services.ShoppingCartAPI.Models;
using Microservices.App.Services.ShoppingCartAPI.Models.Dtos;
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
        private readonly AppDbContext dbContext;

        public ShoppingCartAPIController(AppDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.responseDto = new ResponseDto();
        }

        [HttpPost]
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
    }
}
