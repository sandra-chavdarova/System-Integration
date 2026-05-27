// TODO: Implement ApiKeyAuthMiddleware
// - Only apply to paths starting with /api/external
// - Check X-Api-Key header
// - If missing: 401 "API key is required"
// - If invalid: 401 "Invalid API key"
// - Read expected key from IOptions<ApiKeySettings>

using Domain.Config;
using Microsoft.Extensions.Options;

namespace Web.Middleware;

public class ApiKeyAuthMiddleware
{
    private readonly RequestDelegate _next;

    public ApiKeyAuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IOptions<ApiKeySettings> _settings)
    {
        if (!context.Request.Path.StartsWithSegments("/api/external"))
        {
            await _next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue("X-Api-Key", out var key))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new
            {
                Error = "API key is required"
            });
            return;
        }

        if (key != _settings.Value.ApiKey)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new
            {
                Error = "Invalid API key"
            });
            return;
        }

        await _next(context);
    }
}