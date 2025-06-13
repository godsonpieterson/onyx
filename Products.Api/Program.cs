using Bogus;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Products.Api.Configuration;
using Products.Api.DataAccess.Entities;
using Products.Api.DataAccess.Repositories;
using Products.Api.Models.Enums;
using Scalar.AspNetCore;

public class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

        builder.Services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            options.TokenValidationParameters.ValidateAudience = false;
        });

        builder.Services.AddControllers();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi(options => options.AddDocumentTransformer<OpenApiSecuritySchemeTransformer>());

        builder.Services
            .AddProblemDetails(options =>
                options.CustomizeProblemDetails = ctx =>
                {
                    ctx.ProblemDetails.Extensions.Add("instance", $"{ctx.HttpContext.Request.Method} {ctx.HttpContext.Request.Path}");
                }); builder.Services.AddProblemDetails();

        builder.Services.AddExceptionHandler<ExceptionToProblemDetailsHandler>();

        builder.Services.AddScoped<IProductsRepository, ProductsRepository>();

        builder.Services.AddDbContext<ProductsDbContext>(options => options
            .UseInMemoryDatabase("Products Database")
            .UseAsyncSeeding(async (context, _, ct) =>
            {
                var faker = new Faker<ProductEntity>()
                    .UseSeed(1)
                    .RuleFor(p => p.Name, f => f.Random.Word())
                    .RuleFor(p => p.Colour, f => f.PickRandom<ColourEnum>());

                var products = faker.Generate(5);

                context.Set<ProductEntity>().AddRange(products);
                await context.SaveChangesAsync(ct);
            }));

        builder.Services.AddHealthChecks();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();

            app.MapScalarApiReference();
        }

        app.UseStatusCodePages();

        app.UseExceptionHandler();

        app.UseHttpsRedirection();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        app.MapHealthChecks("/_health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        await using (var serviceScope = app.Services.CreateAsyncScope())
        await using (var dbContext = serviceScope.ServiceProvider.GetRequiredService<ProductsDbContext>())
        {
            await dbContext.Database.EnsureCreatedAsync();
        }

        app.Run();
    }
}