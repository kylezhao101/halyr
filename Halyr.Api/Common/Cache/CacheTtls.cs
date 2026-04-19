namespace Halyr.Api.Common.Cache;

public static class CacheTtls
{
    public static readonly TimeSpan FeatureFlag = TimeSpan.FromMinutes(15);
    public static readonly TimeSpan EnvironmentConfig = TimeSpan.FromMinutes(10);
}