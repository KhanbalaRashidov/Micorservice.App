using Microservices.App.Services.AuthAPI.Data;
using Microservices.App.Services.AuthAPI.Models;
using Microservices.App.Services.AuthAPI.Models.Dtos;
using Microservices.App.Services.AuthAPI.Service.Abstracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Microservices.App.Services.AuthAPI.Service
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext dbContext;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IJwtTokenGenerator jwtTokenGenerator;

        public AuthService(AppDbContext dbContext, UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager, IJwtTokenGenerator jwtTokenGenerator)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<string> Register(RegistrationRequestDto registrationRequestDto)
        {
            ApplicationUser user = new()
            {
                UserName = registrationRequestDto.Email,
                Email = registrationRequestDto.Email,
                NormalizedEmail = registrationRequestDto.Email.ToUpper(),
                Name = registrationRequestDto.Name,
                PhoneNumber = registrationRequestDto.PhoneNumber
            };

            try
            {
                var result = await this.userManager.CreateAsync(user, registrationRequestDto.Password);

                if (result.Succeeded)
                {
                    var userToReturn = this.dbContext.ApplicationUsers.First(user => user.UserName == registrationRequestDto.Email);

                    UserDto userDto = new()
                    {
                        Email = userToReturn.Email,
                        ID = userToReturn.Id,
                        Name = userToReturn.Name,
                        PhoneNumber = userToReturn.PhoneNumber
                    };

                    return "";
                }
                else
                {
                    return result.Errors.FirstOrDefault().Description;
                }
            }
            catch (Exception e)
            {

            }

            return "Error Encountered";
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            var user = await this.dbContext.ApplicationUsers.FirstOrDefaultAsync(u =>
                u.UserName.ToLower() == loginRequestDto.UserName.ToLower());

            var isValid = await this.userManager.CheckPasswordAsync(user, loginRequestDto.Password);

            if (user == null || isValid == false)
            {
                return new LoginResponseDto() { User = null, Token = "" };
            }

            var token = this.jwtTokenGenerator.GenerateJwtToken(user);

            var userDto = new UserDto()
            {
                ID = user.Id,
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };

            var loginResponseDto = new LoginResponseDto()
            {
                User = userDto,
                Token = token 
            };

            return loginResponseDto;
        }

        public async Task<bool> AssignRole(string email, string roleName)
        {
            var user= this.dbContext.ApplicationUsers.FirstOrDefault(u=>u.Email.ToLower() == email.ToLower());

            if (user!= null)
            {
                if (!this.roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
                {
                    this.roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
                }

                await this.userManager.AddToRoleAsync(user, roleName);

                return true;
            }

            return false;
        }
    }
}
