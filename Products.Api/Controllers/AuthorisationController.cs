using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.Text;

namespace Products.Api.Controllers
{
    [Route("api/authorisation")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class AuthorisationController(IConfiguration configuration) : ControllerBase
    {
        private readonly IConfiguration _configuration = configuration;

        [HttpPost]
        [Route("token")]
        public async Task<IActionResult> Token()
        {
            var authHeader = Request.Headers.Authorization;
            var authHeaderInfo = authHeader.ToString().Split(' ');
            var base64TokenInfo = Convert.FromBase64String(authHeaderInfo[1]);
            var base64DecodedCIAndSecret = Encoding.UTF8.GetString(base64TokenInfo);
            var app = ConfidentialClientApplicationBuilder
                .Create(base64DecodedCIAndSecret.Split(':')[0])
                .WithClientSecret(base64DecodedCIAndSecret.Split(':')[1])
                .WithAuthority($"{_configuration.GetValue<string>("AzureAd:Instance")}{_configuration.GetValue<string>("AzureAd:TenantId")}")
                .Build();
            var tokenResult = await app.AcquireTokenForClient([_configuration.GetValue<string>("AzureAd:Scope")!]).ExecuteAsync();

            return Ok(new { access_token = tokenResult.AccessToken });
        }
    }
}
