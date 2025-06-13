using Microsoft.AspNetCore.OpenApi;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;

namespace Products.Api.Configuration
{
    public class OpenApiSecuritySchemeTransformer(IConfiguration configuration) : IOpenApiDocumentTransformer
    {
        private readonly IConfiguration _configuration = configuration;

        public Task TransformAsync(
            OpenApiDocument document,
            OpenApiDocumentTransformerContext context,
            CancellationToken cancellationToken)
        {
            var securitySchema =
                new OpenApiSecurityScheme
                {
                    Scheme = "OAuth2",                    
                    Flows = 
                        new OpenApiOAuthFlows
                        {
                            ClientCredentials = 
                                new OpenApiOAuthFlow
                                {
                                    TokenUrl = new Uri(_configuration.GetValue<string>("AzureAd:TokenUrl")!),
                                    Scopes = new Dictionary<string, string> { { _configuration.GetValue<string>("AzureAd:Scope")!, "default" }}
                                }
                        },
                    In = ParameterLocation.Header,
                    Name = HeaderNames.Authorization,
                    Type = SecuritySchemeType.OAuth2
                };

            var securityRequirement = 
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = "OAuth2",
                                Type = ReferenceType.SecurityScheme,
                            },
                        },
                        Array.Empty<string>()
                    }
                };

            document.SecurityRequirements.Add(securityRequirement);
            document.Components = 
                new OpenApiComponents()
                {
                    SecuritySchemes = new Dictionary<string, OpenApiSecurityScheme>()
                    {
                        { "OAuth2", securitySchema }
                    }
                };

            return Task.CompletedTask;
        }
    }
}
