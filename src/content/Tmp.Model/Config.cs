namespace Tmp.Model;

public static class Config
{
    static Config()
    {
        var env = Environment.GetEnvironmentVariables();

        DBConnection = env[Constant.ConfigKey.DBCon]?.ToString()
            ?? throw new ArgumentNullException(Constant.Message.NoDBConnection);
        TenantId = env[Constant.ConfigKey.TenantId]?.ToString()
            ?? throw new ArgumentNullException(Constant.Message.NoTenantId);
    }

    public static string DBConnection { get; private set; }

    public static string TenantId { get; private set; }
}
