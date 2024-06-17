using Microsoft.AspNetCore.Authorization;

public class ApiKeyAuthorizationAttribute : AuthorizeAttribute
{
    public const string ApiKeyHeaderName = "X-Api-Key";
}