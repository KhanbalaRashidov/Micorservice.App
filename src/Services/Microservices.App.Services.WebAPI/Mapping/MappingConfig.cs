using AutoMapper;
using Microservices.App.Services.WebAPI.Models;
using Microservices.App.Services.WebAPI.Models.Dtos;

namespace Microservices.App.Services.WebAPI.Mapping
{
    public class MappingConfig 
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<CouponDto, Coupon>();
                config.CreateMap<Coupon, CouponDto>();
            });

            return mappingConfig;
        }
    }
}
