namespace Tmp.Api;

using Tmp.Interface.Service;
using Tmp.Model;
using Tmp.Repository;
using Tmp.Service;

internal static class ServiceExt
{
    public static IServiceCollection RegistService(this IServiceCollection services)
    {
        // Register service
        services.AddScoped<IUserService, UserService>();

        // Register repository
        services.AddTmpPersistence();

        // Others
        services.AddHttpContextAccessor();

        return services;
    }

    public static IServiceCollection AllowCORS(this IServiceCollection services)
    {
        var env = Environment.GetEnvironmentVariables();
        string? originsStr = env[Constant.ConfigKey.AllowedOrigins]?.ToString()
            ?? throw new ArgumentNullException(nameof(originsStr));
        string[] origins = originsStr.Split(';');
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
