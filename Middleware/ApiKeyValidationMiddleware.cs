using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

public class ApiKeyValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ApiKeys _apiKeys;

    public ApiKeyValidationMiddleware(RequestDelegate next, IOptions<ApiKeys> apiKeys)
    {
        _next = next;
        _apiKeys = apiKeys.Value;
    }

    public async Task Invoke(HttpContext context)
    {
        string apiKey = context.Request.Headers[ApiKeyAuthorizationAttribute.ApiKeyHeaderName];

        if (apiKey == _apiKeys.DefaultKey || apiKey == _apiKeys.SpecialKey)
        {
            await _next(context);
        }
        else
        {
            context.Response.StatusCode = 401; // Unauthorized
            await context.Response.WriteAsync("Invalid API key.");
        }
    }
}