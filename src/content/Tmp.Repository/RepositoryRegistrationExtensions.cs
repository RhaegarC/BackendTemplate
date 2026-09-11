namespace Tmp.Repository;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Tmp.Interface.Repository;
using Tmp.Model;

public static class RepositoryRegistrationExtensions
{
    public static IServiceCollection AddTmpPersistence(this IServiceCollection services)
    {
        services.AddScoped<AuditSaveChangesInterceptor>();

        services.AddDbContext<TmpContext>((serviceProvider, options) =>
        {
            options.UseNpgsql(Config.DBConnection)
                .AddInterceptors(serviceProvider.GetRequiredService<AuditSaveChangesInterceptor>());
        });

        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}
