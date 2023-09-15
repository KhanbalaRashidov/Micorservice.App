using AutoMapper;
using Microservices.App.Services.WebAPI.Data;
using Microservices.App.Services.WebAPI.Models;
using Microservices.App.Services.WebAPI.Models.Dtos;
using Microsoft.AspNetCore.Mvc;

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

                responseDto.Result = this.mapper.Map<IEnumerable<CouponDto>>(coupons);
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
        
        [HttpGet]
        [Route("GetByCode/{code}")]
        public ResponseDto GetByCode(string code)
        {
            try
            {
                var coupon = this.appDbContext.Coupons.First(x => x.CouponCode.ToLower()==code.ToLower());
                
                responseDto.Result = this.mapper.Map<CouponDto>(coupon);
            }
            catch (Exception ex)
            {
                responseDto.IsSuccess = false;
                responseDto.Message = ex.Message;
            }

            return responseDto;
        }
        
        [HttpPost]
        public ResponseDto Post([FromBody] CouponDto couponDto)
        {
            try
            {
                var coupon = this.mapper.Map<Coupon>(couponDto);
                this.appDbContext.Coupons.Add(coupon);
                this.appDbContext.SaveChanges();
                
                responseDto.Result = this.mapper.Map<CouponDto>(coupon);
            }
            catch (Exception ex)
            {
                responseDto.IsSuccess = false;
                responseDto.Message = ex.Message;
            }

            return responseDto;
        }
        
        [HttpPut]
        public ResponseDto Put([FromBody] CouponDto couponDto)
        {
            try
            {
                var coupon = this.mapper.Map<Coupon>(couponDto);
                this.appDbContext.Coupons.Update(coupon);
                this.appDbContext.SaveChanges();
                
                responseDto.Result = this.mapper.Map<CouponDto>(coupon);
            }
            catch (Exception ex)
            {
                responseDto.IsSuccess = false;
                responseDto.Message = ex.Message;
            }

            return responseDto;
        }
        
        [HttpDelete]
        public ResponseDto Delete(int id)
        {
            try
            {
                var coupon = this.appDbContext.Coupons.First(coupon=>coupon.CouponId==id);
                this.appDbContext.Coupons.Remove(coupon);
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
