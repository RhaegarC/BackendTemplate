namespace Tmp.Repository;

using Microsoft.EntityFrameworkCore;
using Model.DatabaseEntity;

public class TmpContext : DbContext
{
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
    }
}
