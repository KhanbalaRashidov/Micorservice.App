using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microservices.App.Services.AuthAPI.Models;
using Microservices.App.Services.AuthAPI.Service.Abstracts;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Microservices.App.Services.AuthAPI.Service
{
    public class JwtTokenGenerator:IJwtTokenGenerator
    {
        private readonly JwtOptions jwtOptions;

        public JwtTokenGenerator(IOptions<JwtOptions> jwtOptions)
        {
            this.jwtOptions = jwtOptions.Value;
        }

        public string GenerateJwtToken(ApplicationUser applicationUser, IEnumerable<string> roles)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.UTF8.GetBytes(this.jwtOptions.Secret);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, applicationUser.Email),
                new Claim(JwtRegisteredClaimNames.Sub, applicationUser.Id),
                new Claim(JwtRegisteredClaimNames.Name, applicationUser.UserName.ToString())
            };

            claims.AddRange(roles.Select(role=> new Claim(ClaimTypes.Role, role)));

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Audience = this.jwtOptions.Audience,
                Issuer = this.jwtOptions.Issuer,
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(3),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);

        }
    }
}
