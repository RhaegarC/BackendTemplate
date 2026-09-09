namespace Tmp.Model.DatabaseEntity;

public class AuditLog
{
    public int Id { get; set; }

    /// <summary>
    /// Entra Object ID
    /// </summary>
    public string Actor { get; set; }

    /// <summary>
    /// Optional: user's display name
    /// </summary>
    public string? ActorName { get; set; }

    /// <summary>
    /// "Users", "Orders", etc.
    /// </summary>
    public string TableName { get; set; }
    
    /// <summary>
    /// Primary key value (as string)
    /// </summary>
    public string EntityId { get; set; }
    
    /// <summary>
    /// "INSERT", "UPDATE", "DELETE"
    /// </summary>
    public string Action { get; set; }

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
