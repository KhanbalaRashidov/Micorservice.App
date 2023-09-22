using Microservices.App.Services.AuthAPI.Models.Dtos;
using Microservices.App.Services.AuthAPI.Service.Abstracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Microservices.App.Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly IAuthService authService;
        protected ResponseDto responseDto;

        public AuthAPIController(IAuthService authService)
        {
            this.authService = authService;
            this.responseDto = new();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDto registrationRequestDto)
        {
            var errorMessage = await this.authService.Register(registrationRequestDto);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                this.responseDto.IsSuccess = false;
                this.responseDto.Message = errorMessage;

                return BadRequest(this.responseDto);
            }
            return Ok(this.responseDto);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login()
        {
            return Ok();
        }
    }
}
