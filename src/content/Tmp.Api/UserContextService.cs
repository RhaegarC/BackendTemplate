namespace Tmp.Api;

using Tmp.Interface.Service;

public class UserContextService : IUserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContextService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        GetCurrentUserInfo();
    }

    /// <summary>
    /// Entra Object ID for the user
    /// </summary>
    public string? EntraObjectId { get; }

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

    private string? GetCurrentUserInfo()
    {
        // TODO: Implement logic to retrieve the current user's info from the HTTP context or authentication token.
        return string.Empty;
    }
}
