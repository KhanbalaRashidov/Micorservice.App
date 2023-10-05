using AutoMapper;
using Micorservice.App.Services.ProductAPI.Models;
using Micorservice.App.Services.ProductAPI.Models.Dtos;

namespace Micorservice.App.Services.ProductAPI.Mapping
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<ProductDto, Product>().ReverseMap();
            });
            return mappingConfig;
        }
    }
}
