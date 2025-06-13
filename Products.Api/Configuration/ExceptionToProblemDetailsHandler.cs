using Microsoft.AspNetCore.Diagnostics;
using Products.Api.Controllers;

namespace Products.Api.Configuration
{
    public class ExceptionToProblemDetailsHandler(
        ILogger<ProductsController> logger,
        IProblemDetailsService problemDetailsService) : IExceptionHandler
    {
        private readonly ILogger<ProductsController> _logger = logger;
        private readonly IProblemDetailsService _problemDetailsService = problemDetailsService;

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "An error occurred");

            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails =
                {
                    Title = "An error occurred",
                    Detail = exception.Message,
                    Type = exception.GetType().Name,
                },
                Exception = exception
            });
        }
    }
}
