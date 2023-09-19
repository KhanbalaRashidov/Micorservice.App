using Microservices.App.Web.Models;
using Microservices.App.Web.Service.Abstract;
using Microservices.App.Web.Utility;

namespace Microservices.App.Web.Service;

public class CouponService : ICouponService
{
    private readonly IBaseService baseService;

    public CouponService(IBaseService baseService)
    {
        this.baseService = baseService;
    }

    public async Task<ResponseDto?> GetCouponAsync(string couponCode)
    {
        return await this.baseService.SendAsync(new RequestDto()
        {
            ApiType = SD.ApiType.GET,
            Url = SD.CouponAPIBase+"/api/coupon/GetByCode"+couponCode
        });
    }

    public async Task<ResponseDto?> GetAllCouponsAsync()
    {
        return await this.baseService.SendAsync(new RequestDto()
        {
            ApiType = SD.ApiType.GET,
            Url = SD.CouponAPIBase+"/api/coupon"
        });
    }

    public async Task<ResponseDto?> GetCouponByIdAsync(int id)
    {
        return await this.baseService.SendAsync(new RequestDto()
        {
            ApiType = SD.ApiType.GET,
            Url = SD.CouponAPIBase+"/api/coupon/"+id
        });
    }

    public async Task<ResponseDto?> CreateCouponAsync(CouponDto couponDto)
    {
        return await this.baseService.SendAsync(new RequestDto()
        {
            ApiType = SD.ApiType.POST,
            Data = couponDto,
            Url = SD.CouponAPIBase+"/api/coupon"
        });
    }

    public async Task<ResponseDto?> UpdateCouponAsync(CouponDto couponDto)
    {
        return await this.baseService.SendAsync(new RequestDto()
        {
            ApiType = SD.ApiType.PUT,
            Data = couponDto,
            Url = SD.CouponAPIBase+"/api/coupon"
        });
    }

    public async Task<ResponseDto?> DeleteCouponAsync(int id)
    {
        return await this.baseService.SendAsync(new RequestDto()
        {
            ApiType = SD.ApiType.DELETE,
            Url = SD.CouponAPIBase+"/api/coupon/"+id
        });
    }
}