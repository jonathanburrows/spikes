using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AccessCodeClient.Http
{
    public class OidcHttpClientFactory
    {
        private IHttpContextAccessor HttpContextAccessor { get; }

        public OidcHttpClientFactory(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor;
        }

        public async Task<HttpClient> ConstructAsync(HttpContext httpContext)
        {

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var accessToken = await httpContext.Authentication.GetTokenAsync("access_token");
            if (accessToken != null)
            {
                httpClient.SetBearerToken(accessToken);
            }

            return httpClient;
        }
    }
}
