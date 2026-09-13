namespace Tmp.Interface.Service;

public interface IUserContextService
{
    /// <summary>
    /// Entra Object ID for the user. Null when the request is unauthenticated, or when
    /// there is no request in flight at all.
    /// </summary>
    string? EntraObjectId { get; }

    /// <summary>
    /// True when this service was resolved inside an HTTP request. False for startup and
    /// background work, which have no request to attribute a change to.
    /// </summary>
    bool HasActiveRequest { get; }

    /// <summary>
    /// Optional: user's display name
    /// </summary>
    string? ActorName { get; set; }

    // Context
    /// <summary>
    /// From HTTP context
    /// </summary>
    string? IpAddress { get; set; }

    /// <summary>
    /// From HTTP context
    /// </summary>
    string? UserAgent { get; set; }

    /// <summary>
    /// For tracing across services
    /// </summary>
    string? CorrelationId { get; set; }

}
