using Halyr.Api.DTOs;
using Halyr.Api.Enums;

public interface IFeatureFlagCache
{
    Task<EnvironmentConfigResponseDTO?> GetEnvironmentConfigAsync(string flagKey, EnvironmentType environmentKey);
    Task SetEnvironmentConfigAsync(string flagKey, EnvironmentType environmentKey, EnvironmentConfigResponseDTO config, TimeSpan ttl);

    Task InvalidateEnvironmentConfigAsync(string flagKey, EnvironmentType environmentKey);

    Task<FlagResponseDTO?> GetFeatureFlagAsync(string flagKey);
    Task SetFeatureFlagAsync(string flagKey, FlagResponseDTO flag, TimeSpan ttl);
    Task InvalidateFeatureFlagAsync(string flagKey);

    Task InvalidateEnvironmentListAsync(EnvironmentType environmentKey);

    Task InvalidateAfterEnvironmentConfigWriteAsync(string flagKey, EnvironmentType environmentKey);

}