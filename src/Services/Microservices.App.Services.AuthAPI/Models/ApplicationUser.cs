using Microsoft.AspNetCore.Identity;

namespace Microservices.App.Services.AuthAPI.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string Name { get; set; }
    }
}
