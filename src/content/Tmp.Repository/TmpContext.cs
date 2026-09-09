namespace Tmp.Repository;

using Microsoft.EntityFrameworkCore;
using Model.DatabaseEntity;

public class TmpContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>();
        base.OnModelCreating(modelBuilder);
    }
}
