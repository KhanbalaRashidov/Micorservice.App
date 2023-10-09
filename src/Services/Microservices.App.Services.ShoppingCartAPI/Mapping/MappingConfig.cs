using AutoMapper;
using Microservices.App.Services.ShoppingCartAPI.Models;
using Microservices.App.Services.ShoppingCartAPI.Models.Dtos;

namespace Microservices.App.Services.ShoppingCartAPI.Mapping
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<CartDetailsDto, CartDetails>().ReverseMap();
                config.CreateMap<CartHeaderDto, CartHeader>().ReverseMap();
            });
            return mappingConfig;
        }
    }
}
