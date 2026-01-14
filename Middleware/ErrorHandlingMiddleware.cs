using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext ctx)
        {
            try
            {
                await _next(ctx);

                if (ctx.Response.HasStarted)
                    return;

                if (ctx.Response.StatusCode == StatusCodes.Status403Forbidden)
                {
                    ctx.Response.ContentType = "application/json";

                    await ctx.Response.WriteAsJsonAsync(new
                    {
                        status = 403,
                        error = "Acesso negado",
                        message = "Somente usuários administradores podem acessar este recurso"
                    });
                }

                if (ctx.Response.StatusCode == StatusCodes.Status401Unauthorized)
                {
                    ctx.Response.ContentType = "application/json";

                    await ctx.Response.WriteAsJsonAsync(new
                    {
                        status = 401,
                        error = "Não autenticado",
                        message = "Token ausente, inválido ou expirado"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred.");

                if (ctx.Response.HasStarted)
                    return;

                ctx.Response.ContentType = "application/json";
                ctx.Response.StatusCode = ex switch
                {
                    ApplicationException => StatusCodes.Status400BadRequest,
                    KeyNotFoundException => StatusCodes.Status404NotFound,
                    _ => StatusCodes.Status500InternalServerError
                };

                await ctx.Response.WriteAsJsonAsync(new
                {
                    status = ctx.Response.StatusCode,
                    error = ex.Message
                });
            }
        }
    }

}
