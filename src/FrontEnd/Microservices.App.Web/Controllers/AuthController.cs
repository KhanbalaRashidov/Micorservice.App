using Microservices.App.Web.Models;
using Microservices.App.Web.Service.Abstract;
using Microservices.App.Web.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Microservices.App.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService authService;
        private readonly ITokenProvider tokenProvider;

        public AuthController(IAuthService audienceService, ITokenProvider tokenProvider)
        {
            this.authService = audienceService;
            this.tokenProvider = tokenProvider;
        }

        [HttpGet]
        public IActionResult Login()
        {
            LoginRequestDto loginRequestDto = null;

            return View(loginRequestDto);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto loginRequestDto)
        {
            var response = await this.authService.LoginAsync(loginRequestDto);

            if (response != null && response.IsSuccess)
            {
                var loginResponse =
                    JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(response.Result));

                await SignInUser(loginResponse);

                this.tokenProvider.SetToken(loginResponse.Token);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["error"] = response.Message;
                
                return View(loginRequestDto);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            var roleList = new List<SelectListItem>()
            {
                new SelectListItem { Text = SD.RoleAdmin, Value = SD.RoleAdmin },
                new SelectListItem { Text = SD.RoleCustomer, Value = SD.RoleCustomer }
            };
            ViewBag.RoleList = roleList;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationRequestDto registrationRequestDto)
        {
            var responseDto = await this.authService.RegisterAsync(registrationRequestDto);
            ResponseDto assignRole = null;

            if (responseDto != null && responseDto.IsSuccess)
            {
                if (string.IsNullOrEmpty(registrationRequestDto.Role))
                {
                    registrationRequestDto.Role = SD.RoleCustomer;
                }

                assignRole = await authService.AssignRoleAsync(registrationRequestDto);

                if (assignRole != null && assignRole.IsSuccess)
                {
                    TempData["success"] = "Registration is successful";

                    return RedirectToAction(nameof(Login));
                }
            }
            else
            {
                TempData["error"] = responseDto.Message;
            }

            var roleList = new List<SelectListItem>()
            {
                new SelectListItem { Text = SD.RoleAdmin, Value = SD.RoleAdmin },
                new SelectListItem { Text = SD.RoleCustomer, Value = SD.RoleCustomer }
            };
            ViewBag.RoleList = roleList;

            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            this.tokenProvider.ClearToken();

            return RedirectToAction("Index", "Home");
        }

        private async Task SignInUser(LoginResponseDto loginResponseDto)
        {
            var handler = new JwtSecurityTokenHandler();

            var jwt = handler.ReadJwtToken(loginResponseDto.Token);

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email,
                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub,
              jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name,
              jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Name).Value));

            identity.AddClaim(new Claim(ClaimTypes.Name,
             jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));

            identity.AddClaim(new Claim(ClaimTypes.Role,
            jwt.Claims.FirstOrDefault(u => u.Type == "role").Value));

            var pricipil = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, pricipil);
        }
    }
}
