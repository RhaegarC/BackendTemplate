namespace Tmp.Interface.Service;

public interface IUserContextService
{
    /// <summary>
    /// Entra Object ID for the user
    /// </summary>
    string? EntraObjectId { get; }

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
