using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Tmp.Interface.Repository;
using Tmp.Model;

namespace Tmp.Repository;

public static class PersistenceExtensions
{
    /// <summary>Registers <see cref="TmpContext"/> (UseNpgsql only when a connection
    /// string is present — preserves no-DB startup for /health and Swagger), the audit
    /// interceptor (§6.1.1), and the five repositories as scoped services by interface.
    /// Column names are camelCase (no underscores), mapped explicitly in the DbContext.</summary>
    public static IServiceCollection AddFmsPersistence(this IServiceCollection services)
    {
        services.AddDbContext<TmpContext>((serviceProvider, options) =>
        {
            options.UseNpgsql(Config.DBConnection);
        });

        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}
