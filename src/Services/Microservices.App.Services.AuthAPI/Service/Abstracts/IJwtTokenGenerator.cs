using Microservices.App.Services.AuthAPI.Models;

namespace Microservices.App.Services.AuthAPI.Service.Abstracts
{
    public interface IJwtTokenGenerator
    {
        string GenerateJwtToken(ApplicationUser applicationUser,IEnumerable<string> roles);
    }
}
