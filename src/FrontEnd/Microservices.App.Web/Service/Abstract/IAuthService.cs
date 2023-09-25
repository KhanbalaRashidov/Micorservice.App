using Microservices.App.Web.Models;

namespace Microservices.App.Web.Service.Abstract
{
    public interface IAuthService
    {
        Task<ResponseDto> LoginAsync(LoginRequestDto  loginRequestDto);
        Task<ResponseDto> RegisterAsync(RegistrationRequestDto registrationRequestDto);
        Task<ResponseDto> AssignRoleAsync(RegistrationRequestDto registrationRequestDto);

    }
}
