using Microservices.App.Services.ShoppingCartAPI.Models.Dtos;

namespace Microservices.App.Services.ShoppingCartAPI.Service.Abstract
{
    public interface ICouponService
    {
        Task<CouponDto> GetCoupon(string couponCode);
    }
}
