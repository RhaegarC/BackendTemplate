namespace Tmp.Model;

public static class Constant
{
    public static class App
    {
        public const string CORSPolicyName = "AllowSpecificOrigin";
        public const string HealthCheckUrl = "/health";
        public const string SwaggerUrl = "/swagger";
    }

    public static class Message
    {
        public const string NoDBConnection = "Database connection not configured.";
        
        public const string NoTenantId = "Tenant id not configured.";

        public const string Unauthorized = "Unauthorized.";
    }

    public static class ConfigKey
    {
        public const string DBCon = "DbConnection";
        
        public const string TenantId = "TenantId";
        
        public const string AllowedOrigins = "AllowedOrigins";
    }
}
