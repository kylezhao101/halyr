using System.Text.Json;
using Halyr.Api.DTOs;
using StackExchange.Redis;
using Halyr.Api.Enums;

public class RedisFeatureFlagCache : IFeatureFlagCache
{
    private readonly IDatabase _db;
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    public RedisFeatureFlagCache(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }

    private static string EnvKey(string flagKey, EnvironmentType environmentKey)
    {
        return $"featureflag:flag:{flagKey}:env:{NormalizeEnvironmentKey(environmentKey)}";
    }

    private static string FlagKey(string flagKey)
    {
        return $"featureflag:flag:{NormalizeFlagKey(flagKey)}";
    }

    private static string EnvListKey(EnvironmentType environmentKey)
    {
        return $"featureflag:flags:env:{NormalizeEnvironmentKey(environmentKey)}";
    }

    private static string NormalizeFlagKey(string flagKey)
    {
        return flagKey.ToLowerInvariant();
    }

    private static string NormalizeEnvironmentKey(EnvironmentType environmentKey)
    {
        return environmentKey.ToString().ToLowerInvariant();
    }

    public async Task<EnvironmentConfigResponseDTO?> GetEnvironmentConfigAsync(string flagKey, EnvironmentType environmentKey)
    {
        var envConfigJson = await _db.StringGetAsync(EnvKey(NormalizeFlagKey(flagKey), environmentKey));
        if (envConfigJson.IsNullOrEmpty)
            return null;

        return JsonSerializer.Deserialize<EnvironmentConfigResponseDTO>(
            envConfigJson.ToString(), 
            _jsonOptions
        );
    }

    public async Task SetEnvironmentConfigAsync(string flagKey, EnvironmentType environmentKey, EnvironmentConfigResponseDTO envConfig, TimeSpan ttl)
    {
        var envConfigJson = JsonSerializer.Serialize(envConfig, _jsonOptions);
        await _db.StringSetAsync(EnvKey(NormalizeFlagKey(flagKey), environmentKey), envConfigJson, ttl);
    }

    public async Task InvalidateEnvironmentConfigAsync(string flagKey, EnvironmentType environmentKey)
    {
        await _db.KeyDeleteAsync(EnvKey(NormalizeFlagKey(flagKey), environmentKey));
    }

    public async Task<FlagResponseDTO?> GetFeatureFlagAsync(string flagKey)
    {
        var flagJson = await _db.StringGetAsync(FlagKey(flagKey));
        if (flagJson.IsNullOrEmpty)
            return null;

        return JsonSerializer.Deserialize<FlagResponseDTO>(
            flagJson.ToString(), 
            _jsonOptions
        );
    }

    public async Task SetFeatureFlagAsync(string flagKey, FlagResponseDTO flag, TimeSpan ttl)
    {
        var flagJson = JsonSerializer.Serialize(flag, _jsonOptions);
        await _db.StringSetAsync(FlagKey(flagKey), flagJson, ttl);
    }

    public async Task InvalidateFeatureFlagAsync(string flagKey)
    {
        await _db.KeyDeleteAsync(FlagKey(flagKey));
    }

    public async Task InvalidateEnvironmentListAsync(EnvironmentType environmentKey)
    {
        await _db.KeyDeleteAsync(EnvListKey(environmentKey));
    }

    public async Task InvalidateAfterEnvironmentConfigWriteAsync(string flagKey, EnvironmentType environmentKey)
    {
        await Task.WhenAll(
            InvalidateEnvironmentConfigAsync(flagKey, environmentKey),
            InvalidateEnvironmentListAsync(environmentKey),
            InvalidateFeatureFlagAsync(flagKey)
        );
    }

}