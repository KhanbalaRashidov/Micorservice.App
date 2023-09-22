using Microservices.App.Services.AuthAPI.Models.Dtos;

namespace Microservices.App.Services.AuthAPI.Service.Abstracts
{
    public interface IAuthService
    {
        Task<string> Register(RegistrationRequestDto registrationRequestDto);
        Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);
    }
}
