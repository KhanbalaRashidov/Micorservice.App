using Microservices.App.Web.Models;
using Microservices.App.Web.Service.Abstract;
using Microservices.App.Web.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace Microservices.App.Web.Controllers
{
    public class AuthController : Controller
    {
        private  readonly  IAuthService authService;
        private  readonly ITokenProvider tokenProvider;

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

            if (response!=null && response.IsSuccess)
            {
                var loginResponse = 
                    JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(response.Result));

                this.tokenProvider.SetToken(loginResponse.Token);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("CustomError", response.Message);

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

            var roleList = new List<SelectListItem>()
            {
                new SelectListItem { Text = SD.RoleAdmin, Value = SD.RoleAdmin },
                new SelectListItem { Text = SD.RoleCustomer, Value = SD.RoleCustomer }
            };
            ViewBag.RoleList = roleList;

            return View();
        }

        public IActionResult Logout()
        {
            return View();
        }

        private async Task SignInUser(LoginRequestDto loginRequestDto)
        {
           // var handler= new JwtSecurityTokenhandler()
        }
    }
}
