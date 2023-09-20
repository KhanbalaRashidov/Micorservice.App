using System.Net;
using System.Text;
using Microservices.App.Web.Models;
using Microservices.App.Web.Service.Abstract;
using Microservices.App.Web.Utility;
using Newtonsoft.Json;

namespace Microservices.App.Web.Service;

public class BaseService:IBaseService
{
    private readonly IHttpClientFactory httpClientFactory;

    public BaseService(IHttpClientFactory httpClientFactory)
    {
        this.httpClientFactory = httpClientFactory;
        ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
        
    }

    public async Task<ResponseDto?> SendAsync(RequestDto requestDto)
    {
        try
        {
            var client = this.httpClientFactory.CreateClient("Microservice");
            
            HttpRequestMessage message = new();
            message.Headers.Add("Accept", "application/json");

            // token

            message.RequestUri = new Uri(requestDto.Url);
            if (requestDto.Data is not null)
            {
                message.Content = new StringContent(JsonConvert.SerializeObject(requestDto.Data), Encoding.UTF8,
                    "application/json");
            }

            HttpResponseMessage apiResponse = null;

            switch (requestDto.ApiType)
            {
                case SD.ApiType.GET:
                    message.Method = HttpMethod.Get;
                    break;
                case SD.ApiType.POST:
                    message.Method = HttpMethod.Post;
                    break;
                case SD.ApiType.PUT:
                    message.Method = HttpMethod.Put;
                    break;
                case SD.ApiType.DELETE:
                    message.Method = HttpMethod.Delete;
                    break;
            }

            apiResponse = await client.SendAsync(message);

            switch (apiResponse.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    return new() { IsSuccess = false, Message = "Not Found" };
                case HttpStatusCode.Forbidden:
                    return new() { IsSuccess = false, Message = "Access Denied" };
                case HttpStatusCode.Unauthorized:
                    return new() { IsSuccess = false, Message = "Unauthorized" };
                case HttpStatusCode.InternalServerError:
                    return new() { IsSuccess = false, Message = "Internal Server Error" };
                default:
                    var apiContent = await apiResponse.Content.ReadAsStringAsync();
                    var apiResponseDto = JsonConvert.DeserializeObject<ResponseDto>(apiContent);

                    return apiResponseDto;
            }
        }
        catch (Exception ex)
        {
            return new()
            {
                Message = ex.Message.ToString(),
                IsSuccess = false
            };
        }
    }
}