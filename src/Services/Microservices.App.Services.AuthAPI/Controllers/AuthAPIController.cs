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
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var loginResponse = await this.authService.Login(loginRequestDto);

            if (loginResponse.User==null)
            {
                this.responseDto.IsSuccess=false;
                this.responseDto.Message = "Username or password is incorrect";
                
                return BadRequest(this.responseDto);
            }

            this.responseDto.Result=loginResponse;

            return Ok(this.responseDto);
        }

        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] RegistrationRequestDto registrationRequestDto)
        {
            var assignRoleSuccessful = await this.authService.AssignRole(registrationRequestDto.Email,registrationRequestDto.Role.ToUpper());

            if (!assignRoleSuccessful)
            {
                this.responseDto.IsSuccess = false;
                this.responseDto.Message = "Error encountered";

                return BadRequest(this.responseDto);
            }

            return Ok(this.responseDto);
        }
    }
}
