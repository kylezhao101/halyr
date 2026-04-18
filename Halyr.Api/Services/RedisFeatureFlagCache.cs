

using System.Text.Json;
using Halyr.Api.Data;
using Halyr.Api.DTOs;
using StackExchange.Redis;

public class RedisFeatureFlagCache : IFeatureFlagCache
{
    private readonly IDatabase _db;
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    public RedisFeatureFlagCache(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }

    private static string EnvKey(string flagKey, string environmentKey)
    {
        return $"featureflag:flag:{flagKey}:env:{environmentKey}";
    }

    private static string FlagKey(string flagKey)
    {
        return $"featureflag:flag:{flagKey}";
    }

    private static string EnvListKey(string environmentKey)
    {
        return $"featureflag:flags:env:{environmentKey}";
    }

    public async Task<EnvironmentConfigResponseDTO?> GetEnvironmentConfigAsync(string flagKey, string environmentKey)
    {
        var envConfigJson = await _db.StringGetAsync(EnvKey(flagKey, environmentKey));
        if (envConfigJson.IsNullOrEmpty)
            return null;

        return JsonSerializer.Deserialize<EnvironmentConfigResponseDTO>(
            envConfigJson.ToString(), 
            _jsonOptions
        );
    }

    public async Task SetEnvironmentConfigAsync(string flagKey, string environmentKey, EnvironmentConfigResponseDTO envConfig, TimeSpan ttl)
    {
        var envConfigJson = JsonSerializer.Serialize(envConfig, _jsonOptions);
        await _db.StringSetAsync(EnvKey(flagKey, environmentKey), envConfigJson, ttl);
    }

    public async Task InvalidateEnvironmentConfigAsync(string flagKey, string environmentKey)
    {
        await _db.KeyDeleteAsync(EnvKey(flagKey, environmentKey));
    }

    public async Task<FlagResponseDTO?> GetFeatureFlagAsync(string flagkey)
    {
        var flagJson = await _db.StringGetAsync(FlagKey(flagkey));
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

    public async Task InvalidateEnvironmentListAsync(string environmentKey)
    {
        await _db.KeyDeleteAsync(EnvListKey(environmentKey));
    }

}