namespace Tmp.Repository;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tmp.Interface.Infrastructure;
using Tmp.Model.DatabaseEntity;

internal sealed class AuditSaveChangesInterceptor(IUserContextService userContextService) : SaveChangesInterceptor
{
    /// <summary>Attribution for work with no request behind it — startup, background jobs.</summary>
    private const string SystemActor = "system";

    /// <summary>Attribution for a request that arrived without an authenticated user.</summary>
    private const string AnonymousActor = "anonymous";

    private readonly IUserContextService _currentUser = userContextService;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        WriteIndented = false
    };

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        // Capture audit logs BEFORE save
        var auditLogs = CaptureAuditLogs(eventData.Context);

        // Add logs to the SAME context
        if (eventData.Context is TmpContext dbContext)
        {
            dbContext.AuditLogs.AddRange(auditLogs);
        }

        // Let the save happen
        var saveResult = base.SavingChangesAsync(eventData, result, cancellationToken);

        // 3. AFTER save, we now have the generated IDs (if any) - but AuditLog doesn't need them
        // Optionally save audit logs in a separate transaction or after the main save

        return saveResult;
    }

    private List<AuditLog> CaptureAuditLogs(DbContext? context)
    {
        if (context == null) return new List<AuditLog>();

        var logs = new List<AuditLog>();
        var now = DateTimeOffset.UtcNow;
        var actor = ResolveActor();

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.Entity is AuditLog) continue;

            if (entry.State == EntityState.Unchanged) continue;

            // The mapped table name is absent for a type that has no table — the column is
            // NOT NULL, so fall back to the CLR type name rather than writing null.
            var tableName = entry.Metadata.GetTableName()
                ?? entry.Metadata.GetDefaultTableName()
                ?? entry.Metadata.ClrType.Name;
            var entityId = GetPrimaryKeyValue(entry);

            var oldValues = entry.State == EntityState.Added
                ? null
                : SerializeEntity(entry, isOriginal: true);

            var newValues = entry.State == EntityState.Deleted
                ? null
                : SerializeEntity(entry, isOriginal: false);

            var log = new AuditLog
            {
                TableName = tableName,
                EntityId = entityId,
                Action = entry.State.ToString(),
                OldValues = oldValues,
                NewValues = newValues,
                ChangedColumns = GetChangedColumns(entry),
                Actor = actor,
                ActorName = _currentUser.ActorName,
                Timestamp = now,
                IpAddress = _currentUser.IpAddress,
                UserAgent = _currentUser.UserAgent,
                CorrelationId = _currentUser.CorrelationId
            };

            logs.Add(log);
        }

        return logs;
    }

    /// <summary>
    /// <see cref="AuditLog.Actor"/> is NOT NULL, so an unidentified caller still needs a
    /// value. A request that arrived unauthenticated and work that ran with no request at
    /// all are different problems — the first is a missing token, the second is expected —
    /// so they get different actors rather than one shared "unknown".
    /// </summary>
    private string ResolveActor()
    {
        string? actor = _currentUser.EntraObjectId;

        if (!string.IsNullOrWhiteSpace(actor))
        {
            return actor;
        }

        return _currentUser.HasActiveRequest ? AnonymousActor : SystemActor;
    }

    private string? GetPrimaryKeyValue(EntityEntry entry)
    {
        var key = entry.Metadata.FindPrimaryKey();
        if (key == null) return null;

        var keyValues = key.Properties
            .Select(p => entry.Property(p.Name).CurrentValue?.ToString())
            .Where(v => v != null);

        return string.Join("-", keyValues);
    }

    private string? SerializeEntity(EntityEntry entry, bool isOriginal)
    {
        // Clone the entity's properties to a dictionary
        var properties = entry.Metadata.GetProperties()
            .Where(p => !p.IsShadowProperty()) // Skip EF shadow properties
            .ToDictionary(
                p => p.Name,
                p => isOriginal
                    ? entry.Property(p.Name).OriginalValue
                    : entry.Property(p.Name).CurrentValue
            );

        // Exclude sensitive fields (optional)
        // properties.Remove("PasswordHash");
        // properties.Remove("RefreshToken");

        return JsonSerializer.Serialize(properties, _jsonOptions);
    }

    private string? GetChangedColumns(EntityEntry entry)
    {
        if (entry.State == EntityState.Added || entry.State == EntityState.Deleted)
            return null;

        var changed = entry.Properties
            .Where(p => p.IsModified)
            .Select(p => p.Metadata.Name)
            .ToList();

        return changed.Any() ? string.Join(", ", changed) : null;
    }
}