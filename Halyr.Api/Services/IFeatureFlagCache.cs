using Halyr.Api.DTOs;

public interface IFeatureFlagCache
{
    Task<EnvironmentConfigResponseDTO?> GetEnvironmentConfigAsync(string flagKey, string environmentKey);
    Task SetEnvironmentConfigAsync(string flagKey, string environmentKey, EnvironmentConfigResponseDTO config, TimeSpan ttl);

    Task InvalidateEnvironmentConfigAsync(string flagKey, string environmentKey);

    Task<FlagResponseDTO?> GetFeatureFlagAsync(string flagKey);
    Task SetFeatureFlagAsync(string flagKey, FlagResponseDTO flag, TimeSpan ttl);
    Task InvalidateFeatureFlagAsync(string flagKey);

    Task InvalidateEnvironmentListAsync(string environmentKey);

}