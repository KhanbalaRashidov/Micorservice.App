using AutoMapper;
using Microservices.App.Services.WebAPI.Data;
using Microservices.App.Services.WebAPI.Models.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Microservices.App.Services.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponAPIController : ControllerBase
    {
        private readonly AppDbContext appDbContext;
        private ResponseDto responseDto;
        private IMapper mapper;

        public CouponAPIController(AppDbContext appDbContext,IMapper mapper)
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
                var coupons = this.appDbContext.Coupons.ToList();

                responseDto.Result = this.mapper.Map<CouponDto>(coupons);
            }
            catch (Exception ex)
            {

                responseDto.IsSuccess = false;
                responseDto.Message= ex.Message;
            }

            return responseDto;
        }

        [HttpGet]
        [Route("{id:int}")]
        public ResponseDto Get(int id)
        {
            try
            {
                var coupon = this.appDbContext.Coupons.First(x => x.CouponId == id);

                responseDto.Result = this.mapper.Map<CouponDto>(coupon);
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
