using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Tmp.Interface.Repository;

namespace Tmp.Repository;

public static class PersistenceExtensions
{
    /// <summary>Registers <see cref="TmpContext"/> (UseNpgsql only when a connection
    /// string is present — preserves no-DB startup for /health and Swagger), the audit
    /// interceptor (§6.1.1), and the five repositories as scoped services by interface.
    /// Column names are camelCase (no underscores), mapped explicitly in the DbContext.</summary>
    /// <param name="services">The service collection to register into.</param>
    /// <param name="connectionString">Resolved by the composition root from configuration;
    /// when null or blank the context is registered without a provider, so the host still
    /// starts for /health and Swagger.</param>
    public static IServiceCollection AddRepositoryPersistence(
        this IServiceCollection services,
        string? connectionString)
    {
        services.AddDbContext<TmpContext>((serviceProvider, options) =>
        {
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                options.UseNpgsql(connectionString);
            }
        });

        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}
