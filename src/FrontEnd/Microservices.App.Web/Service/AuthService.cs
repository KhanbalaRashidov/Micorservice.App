using Microservices.App.Web.Models;
using Microservices.App.Web.Service.Abstract;
using Microservices.App.Web.Utility;

namespace Microservices.App.Web.Service
{
    public class AuthService : IAuthService
    {
        private readonly IBaseService baseService;

        public AuthService(IBaseService baseService)
        {
            this.baseService = baseService;
        }
        public async Task<ResponseDto?> LoginAsync(LoginRequestDto loginRequestDto)
        {
            return await this.baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = loginRequestDto,
                Url = SD.AuthAPIBase + "/api/auth/login"
            });
        }

        public async Task<ResponseDto?> RegisterAsync(RegistrationRequestDto registrationRequestDto)
        {
            return await this.baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = registrationRequestDto,
                Url = SD.AuthAPIBase + "/api/auth/register"
            }, withBearer: false);
        }

        public async Task<ResponseDto?> AssignRoleAsync(RegistrationRequestDto registrationRequestDto)
        {
            return await this.baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = registrationRequestDto,
                Url = SD.AuthAPIBase + "/api/auth/assign-role"
            }, withBearer: false);
        }
    }
}
