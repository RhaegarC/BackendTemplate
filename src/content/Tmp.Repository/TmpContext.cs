namespace Tmp.Repository;

using Microsoft.EntityFrameworkCore;
using Model.DatabaseEntity;

public class TmpContext(DbContextOptions<TmpContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
}
