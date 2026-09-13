namespace Tmp.Api;

using System.Security.Claims;
using Tmp.Interface.Infrastructure;

public class UserContextService : IUserContextService
{
    /// <summary>Entra ID's short claim for the object id.</summary>
    private const string ObjectIdClaim = "oid";

    /// <summary>
    /// The long-form claim older tokens carry for the same value. Both are read, so
    /// whichever the token happens to use is picked up.
    /// </summary>
    private const string ObjectIdSchemaClaim =
        "http://schemas.microsoft.com/identity/claims/objectidentifier";

    private const string CorrelationIdHeader = "X-Correlation-Id";

    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContextService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        GetCurrentUserInfo();
    }

    /// <summary>
    /// Entra Object ID for the user
    /// </summary>
    public string? EntraObjectId { get; private set; }

    /// <summary>
    /// True when this service was resolved inside an HTTP request
    /// </summary>
    public bool HasActiveRequest { get; private set; }

    /// <summary>
    /// Optional: user's display name
    /// </summary>
    public string? ActorName { get; set; }

    // Context
    /// <summary>
    /// From HTTP context
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// From HTTP context
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// For tracing across services
    /// </summary>
    public string? CorrelationId { get; set; }

    private void GetCurrentUserInfo()
    {
        HttpContext? httpContext = _httpContextAccessor.HttpContext;

        // No request in flight — startup, or a background job. Everything stays null and
        // the audit trail attributes the change to "system" rather than to a user.
        if (httpContext is null)
        {
            return;
        }

        HasActiveRequest = true;

        ClaimsPrincipal user = httpContext.User;
        if (user.Identity?.IsAuthenticated == true)
        {
            EntraObjectId = user.FindFirst(ObjectIdClaim)?.Value
                ?? user.FindFirst(ObjectIdSchemaClaim)?.Value;
            ActorName = user.FindFirst("name")?.Value
                ?? user.FindFirst("preferred_username")?.Value;
        }

        IpAddress = httpContext.Connection.RemoteIpAddress?.ToString();
        UserAgent = httpContext.Request.Headers.UserAgent.FirstOrDefault();

        // Prefer a caller-supplied id so a trace survives across services; fall back to
        // ASP.NET's per-request identifier so the column is never empty.
        CorrelationId = httpContext.Request.Headers[CorrelationIdHeader].FirstOrDefault()
            ?? httpContext.TraceIdentifier;
    }
}
