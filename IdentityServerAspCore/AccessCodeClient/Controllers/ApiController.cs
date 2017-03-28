using AccessCodeClient.Http;
using AccessCodeClient.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace AccessCodeClient.Controllers
{
    [Route("[controller]")]
    public class ApiController : ControllerBase
    {
        private ClientOptions ClientOptions { get; }
        private OidcHttpClientFactory HttpClientFactory { get; }

        public ApiController(IOptions<ClientOptions> clientOptions, OidcHttpClientFactory httpClientFactory)
        {
            ClientOptions = clientOptions.Value;
            HttpClientFactory = httpClientFactory;
        }

        [HttpGet("{entityName}/{id}")]
        public async Task<object> GetAsync(string entityName, int id)
        {
            var url = $"{ClientOptions.ResourceServerUrl}{Request.Path}";
            var token = await HttpContext.Authentication.GetTokenAsync("access_token");
            using (var client = new HttpClient())
            {
                client.SetBearerToken(token);
                var response = await client.GetStringAsync(url);
                return JsonConvert.DeserializeObject(response);
            }
        }

        [HttpGet("{entityName}")]
        public async Task<object> GetAsync(string entityName)
        {
            var url = $"{ClientOptions.ResourceServerUrl}{Request.Path}";
            var token = await HttpContext.Authentication.GetTokenAsync("access_token");
            using (var client = new HttpClient())
            {
                client.SetBearerToken(token);
                var response = await client.GetStringAsync(url);
                return JsonConvert.DeserializeObject(response);
            }
        }
    }
}
