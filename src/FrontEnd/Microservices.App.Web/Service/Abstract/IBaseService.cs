using Microservices.App.Web.Models;

namespace Microservices.App.Web.Service.Abstract;

public interface IBaseService
{
    Task<ResponseDto?> SendAsync(RequestDto requestDto, bool withBearer= true);
}