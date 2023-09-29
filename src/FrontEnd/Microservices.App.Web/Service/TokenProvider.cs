using Microservices.App.Web.Service.Abstract;
using Microservices.App.Web.Utility;
using NuGet.Common;

namespace Microservices.App.Web.Service
{
    public class TokenProvider : ITokenProvider
    {
        private readonly IHttpContextAccessor contextAccessor;

        public TokenProvider(IHttpContextAccessor contextAccessor)
        {
            this.contextAccessor = contextAccessor;
        }

        public void SetToken(string token)
        {
            this.contextAccessor.HttpContext?.Response.Cookies.Append(SD.TokenCookie, token);
        }

        public string? GetToken()
        {
            string token = null;

            var hasToken = this.contextAccessor.HttpContext?.Request.Cookies.TryGetValue(SD.TokenCookie, out token);

            return hasToken is true ? token : null;
        }

        public void ClearToken()
        {
            this.contextAccessor.HttpContext?.Response.Cookies.Delete(SD.TokenCookie);
        }
    }
}
