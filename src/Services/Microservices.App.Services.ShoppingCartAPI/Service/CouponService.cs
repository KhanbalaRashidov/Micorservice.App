using Microservices.App.Services.ShoppingCartAPI.Models.Dtos;
using Microservices.App.Services.ShoppingCartAPI.Service.Abstract;
using Newtonsoft.Json;

namespace Microservices.App.Services.ShoppingCartAPI.Service
{
    public class CouponService : ICouponService
    {
        private readonly IHttpClientFactory httpClientFactory;

        public CouponService(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<CouponDto> GetCoupon(string couponCode)
        {
            var client = this.httpClientFactory.CreateClient("Coupon");
            var responseDto = await client.GetAsync($"/api/coupon/GetByCode/{couponCode}");
            var apiContent = await responseDto.Content.ReadAsStringAsync();
            var response= JsonConvert.DeserializeObject<ResponseDto>(apiContent);

            if (response.IsSuccess)
            {
                return JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(response.Result));
            }

            return new CouponDto();
        }
    }
}
