using Microservices.App.Services.ShoppingCartAPI.Models.Dtos;

namespace Microservices.App.Services.ShoppingCartAPI.Service.Abstract
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProductsAsync();
    }
}
