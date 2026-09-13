namespace Tmp.Model.DatabaseEntity;

public class AuditLog
{
    public int Id { get; set; }

    /// <summary>
    /// Entra Object ID of whoever made the change, or "system" for background work and
    /// "anonymous" for an unauthenticated request. Never null.
    /// </summary>
    public required string Actor { get; set; }

    /// <summary>
    /// Optional: user's display name
    /// </summary>
    public string? ActorName { get; set; }

    /// <summary>
    /// "Users", "Orders", etc. Never null.
    /// </summary>
    public required string TableName { get; set; }

    /// <summary>
    /// Primary key value (as string). Null for an insert whose key the database generates
    /// — there is nothing to record until after the save completes. See IM-14.
    /// </summary>
    public string? EntityId { get; set; }

    /// <summary>
    /// "INSERT", "UPDATE", "DELETE". Never null.
    /// </summary>
    public required string Action { get; set; }

    /// <summary>
    /// JSON of before-state (or null for INSERT)
    /// </summary>
    public string? OldValues { get; set; }
    
    /// <summary>
    /// JSON of after-state (or null for DELETE)
    /// </summary>
    public string? NewValues { get; set; }
    
    /// <summary>
    /// Optional: which columns actually changed
    /// </summary>
    public string? ChangedColumns { get; set; }

    /// <summary>
    /// When the action occurred
    /// </summary>
    public DateTimeOffset Timestamp { get; set; }

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
}
