namespace Tmp.Api.MiddleWare;

using Microsoft.Extensions.Logging;
using Tmp.Model;

internal sealed class AuthMiddleWare(RequestDelegate next, ILogger<AuthMiddleWare> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogInformation($"Request path: {context.Request.Path}");
            logger.LogError(ex.Message);
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync(Constant.Message.Unauthorized);
            return;
        }

        await next(context);
    }
}
