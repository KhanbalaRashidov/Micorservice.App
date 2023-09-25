using Microservices.App.Web.Models;
using Microservices.App.Web.Service.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace Microservices.App.Web.Controllers
{
    public class AuthController : Controller
    {
        private  readonly  IAuthService audienceService;

        public AuthController(IAuthService audienceService)
        {
            this.audienceService = audienceService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            LoginRequestDto loginRequestDto = null;

            return View(loginRequestDto);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

      
        public IActionResult Logout()
        {
            return View();
        }
    }
}
