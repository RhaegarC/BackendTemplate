namespace Tmp.Repository;

using Microsoft.EntityFrameworkCore;
using Model.DatabaseEntity;

public class TmpContext(DbContextOptions<TmpContext> options) : DbContext(options)
{
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AuditLog>(entity =>
        {
            // An audit table is usually the fastest-growing table in a system, and it gets
            // read two ways: "what happened to this row" and "what happened around then".
            // Without an index each of those is a sequential scan of the whole history.
            entity.HasIndex(log => new { log.TableName, log.EntityId });
            entity.HasIndex(log => log.Timestamp);

            // Npgsql maps string to text by default, which makes the snapshots
            // write-only. jsonb validates the JSON on the way in and stays queryable
            // afterwards -- the difference between a searchable history and a pile of
            // opaque blobs. Both columns hold JSON or null.
            entity.Property(log => log.OldValues).HasColumnType("jsonb");
            entity.Property(log => log.NewValues).HasColumnType("jsonb");
        });
    }
}
