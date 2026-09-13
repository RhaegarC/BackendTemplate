namespace Tmp.Api;

using Tmp.Interface.Service;
using Tmp.Model;
using Tmp.Repository;
using Tmp.Service;

internal static class ServiceExt
{
    /// <summary>Registers application services together with their dependencies. The
    /// DbContext and the repositories are registered by
    /// <see cref="PersistenceExtensions.AddRepositoryPersistence"/>, so it must run before
    /// the services that consume them.</summary>
    /// <param name="services">The service collection to register into.</param>
    /// <param name="connectionString">Resolved by the composition root from configuration.</param>
    public static IServiceCollection RegistService(this IServiceCollection services, string? connectionString)
    {
        // Register persistence (DbContext + repositories)
        services.AddRepositoryPersistence(connectionString);

        // Register service
        services.AddScoped<IUserService, UserService>();

        // Others

        return services;
    }

    public static IServiceCollection AllowCORS(this IServiceCollection services, IConfiguration configuration)
    {
        string? originsStr = configuration[Constant.ConfigKey.AllowedOrigins];
        if (string.IsNullOrWhiteSpace(originsStr))
        {
            throw new InvalidOperationException(Constant.Message.NoAllowedOrigins);
        }

        string[] origins = originsStr.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        services.AddCors(options =>
        {
            options.AddPolicy(Constant.App.CORSPolicyName,
                builder => builder
                .WithOrigins(origins)
                .AllowAnyHeader()
                .AllowAnyMethod());
        });
        return services;
    }
}
