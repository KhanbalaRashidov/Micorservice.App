using AutoMapper;
using Micorservice.App.Services.ProductAPI.Models;
using Micorservice.App.Services.ProductAPI.Models.Dtos;
using Microservices.App.Services.ProductAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Micorservice.App.Services.ProductAPI.Controllers
{
    [Route("api/product")]
    [ApiController]
    //[Authorize]
    public class ProductAPIController : ControllerBase
    {
        private readonly AppDbContext appDbContext;
        private ResponseDto responseDto;
        private IMapper mapper;

        public ProductAPIController(AppDbContext appDbContext, IMapper mapper)
        {
            this.appDbContext = appDbContext;
            this.responseDto = new ResponseDto();
            this.mapper = mapper;
        }

        [HttpGet]
        public ResponseDto Get()
        {
            try
            {
                var Products = this.appDbContext.Products.ToList();

                responseDto.Result = this.mapper.Map<IEnumerable<ProductDto>>(Products);
            }
            catch (Exception ex)
            {

                responseDto.IsSuccess = false;
                responseDto.Message = ex.Message;
            }

            return responseDto;
        }

        [HttpGet]
        [Route("{id:int}")]
        public ResponseDto Get(int id)
        {
            try
            {
                var Product = this.appDbContext.Products.First(x => x.ProductId == id);

                responseDto.Result = this.mapper.Map<ProductDto>(Product);
            }
            catch (Exception ex)
            {
                responseDto.IsSuccess = false;
                responseDto.Message = ex.Message;
            }

            return responseDto;
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Post([FromBody] ProductDto ProductDto)
        {
            try
            {
                var Product = this.mapper.Map<Product>(ProductDto);
                this.appDbContext.Products.Add(Product);
                this.appDbContext.SaveChanges();

                responseDto.Result = this.mapper.Map<ProductDto>(Product);
            }
            catch (Exception ex)
            {
                responseDto.IsSuccess = false;
                responseDto.Message = ex.Message;
            }

            return responseDto;
        }

        [HttpPut]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Put([FromBody] ProductDto ProductDto)
        {
            try
            {
                var Product = this.mapper.Map<Product>(ProductDto);
                this.appDbContext.Products.Update(Product);
                this.appDbContext.SaveChanges();

                responseDto.Result = this.mapper.Map<ProductDto>(Product);
            }
            catch (Exception ex)
            {
                responseDto.IsSuccess = false;
                responseDto.Message = ex.Message;
            }

            return responseDto;
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Delete(int id)
        {
            try
            {
                var Product = this.appDbContext.Products.First(Product => Product.ProductId == id);
                this.appDbContext.Products.Remove(Product);
                this.appDbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                responseDto.IsSuccess = false;
                responseDto.Message = ex.Message;
            }

            return responseDto;
        }
    }
}
