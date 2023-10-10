using Microservices.App.Services.ShoppingCartAPI.Models.Dtos;
using Microservices.App.Services.ShoppingCartAPI.Service.Abstract;
using Newtonsoft.Json;

namespace Microservices.App.Services.ShoppingCartAPI.Service
{
    public class ProductService : IProductService
    {
        private readonly IHttpClientFactory httpClientFactory;

        public ProductService(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<IEnumerable<ProductDto>> GetProductsAsync()
        {
            var client = this.httpClientFactory.CreateClient("Product");

            var response = await client.GetAsync("/api/product");
            var apiContent = await response.Content.ReadAsStringAsync();
            var responseDto = JsonConvert.DeserializeObject<ResponseDto>(apiContent);

            if (responseDto.IsSuccess)
            {
                return JsonConvert.DeserializeObject<IEnumerable<ProductDto>>(Convert.ToString(responseDto.Result));
            }

            return new List<ProductDto>();
        }
    }
}
