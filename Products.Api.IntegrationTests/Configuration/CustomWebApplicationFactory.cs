using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Products.Api.DataAccess.Repositories;

namespace Products.Api.IntegrationTests.Configuration
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        public Mock<IProductsRepository> MockProductsRepository { get; set; }

        public CustomWebApplicationFactory()
        {
            MockProductsRepository = new Mock<IProductsRepository>();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);

            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton(MockProductsRepository.Object);

                services.AddTransient<IAuthorizationHandler, DisableAuthorizationHandler<IAuthorizationRequirement>>();
            });
        }
    }
}
