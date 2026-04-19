using Halyr.Api.Models;
using Halyr.Api.Enums;
using Halyr.Api.Data;
using Microsoft.EntityFrameworkCore;
using Halyr.Api.DTOs;

namespace Halyr.Api.Services;


public class FeatureFlagService : IFeatureFlagService
{
    private readonly AppDbContext _dbContext;
    private readonly IFeatureFlagCache _cache;

    public FeatureFlagService(AppDbContext dbContext, IFeatureFlagCache cache)
    {
        _dbContext = dbContext;
        _cache = cache;
    }

    private static string NormalizeKey(string key)
    {
        return key.Trim().ToLowerInvariant();
    }

    private static FlagResponseDTO MapToResponse(FeatureFlag flag)
    {
        return new FlagResponseDTO
        {
            Id = flag.Id,
            Key = flag.Key,
            Name = flag.Name,
            Description = flag.Description,
            Environments = flag.Environments.Select(env => new EnvironmentConfigResponseDTO
            {
                Environment = env.Environment,
                Enabled = env.Enabled,
                PercentageRollout = env.PercentageRollout
            }).ToList()
        };
    }

    private static EnvironmentConfigResponseDTO MapToEnvironmentResponse(FeatureFlagEnvironment env)
    {
        return new EnvironmentConfigResponseDTO
        {
            Environment = env.Environment,
            Enabled = env.Enabled,
            PercentageRollout = env.PercentageRollout
        };
    }

    public IEnumerable<FlagResponseDTO> GetAll()
    {
        return _dbContext.FeatureFlags
            .AsNoTracking()
            .Include(flag => flag.Environments)
            .Select(flag => new FlagResponseDTO
            {
                Id = flag.Id,
                Key = flag.Key,
                Name = flag.Name,
                Environments = flag.Environments.Select(env => new EnvironmentConfigResponseDTO
                {
                    Environment = env.Environment,
                    Enabled = env.Enabled,
                    PercentageRollout = env.PercentageRollout
                }).ToList()
            })
            .ToList();
    }

    public FlagResponseDTO? GetByKey(string key)
    {
        var normalizedKey = NormalizeKey(key);
        return _dbContext.FeatureFlags
            .AsNoTracking()
            .Include(flag => flag.Environments)
            .Where(flag => flag.Key == normalizedKey)
            .Select(flag => new FlagResponseDTO
            {   
                Id = flag.Id,
                Key = flag.Key,
                Name = flag.Name,
                Description = flag.Description,
                Environments = flag.Environments.Select(env => new EnvironmentConfigResponseDTO
                {
                    Environment = env.Environment,
                    Enabled = env.Enabled,
                    PercentageRollout = env.PercentageRollout
                }).ToList()
            })
            .FirstOrDefault();
    }

    public FlagResponseDTO Create(CreateFlagRequestDTO request)
    {
        var normalizedKey = NormalizeKey(request.Key);

        var exists = _dbContext.FeatureFlags.Any(f => f.Key == normalizedKey);
        if (exists)
        {
            throw new InvalidOperationException($"A feature flag with key '{normalizedKey}' already exists.");
        }   

        var newFlag = new FeatureFlag
        {
            Id = Guid.NewGuid(),
            Key = normalizedKey,
            Name = request.Name,
            Description = string.IsNullOrWhiteSpace(request.Description)
                ? null
                : request.Description.Trim()
        };

        _dbContext.FeatureFlags.Add(newFlag);
        _dbContext.SaveChanges();

        return GetByKey(newFlag.Key)!;
    }

    public async Task<FlagResponseDTO?> Update(string key, UpdateFlagRequestDTO request)
    {
        var normalizedKey = NormalizeKey(key);
        var flag = _dbContext.FeatureFlags.FirstOrDefault(f => f.Key == normalizedKey);
        if (flag == null)
        {
            return null;
        }

        if (request.Name != null)
        {
            flag.Name = request.Name.Trim();
        }

        if (request.Description != null)
        {
            flag.Description = request.Description.Trim();
        }

        _dbContext.SaveChanges();

        var environmentKeys = flag.Environments.Select(env => env.Environment).ToList();
        await Task.WhenAll(environmentKeys.Select(env => _cache.InvalidateAfterEnvironmentConfigWriteAsync(flag.Key, env)));

        return MapToResponse(flag);
    }

    public async Task<bool> Delete(string key)
    {
        var normalizedKey = NormalizeKey(key);
        var flag = _dbContext.FeatureFlags.Include(f => f.Environments).FirstOrDefault(f => f.Key == normalizedKey);
        if (flag == null)
        {
            return false;
        }

        _dbContext.FeatureFlags.Remove(flag);
        _dbContext.SaveChanges();

        var environmentKeys = flag.Environments.Select(env => env.Environment).ToList();
        await Task.WhenAll(environmentKeys.Select(env => _cache.InvalidateAfterEnvironmentConfigWriteAsync(flag.Key, env)));

        return true;
    }

    public IEnumerable<FlagResponseDTO> GetByEnvironment(EnvironmentType environment)
    {
        return _dbContext.FeatureFlagEnvironments
            .AsNoTracking()
            .Include(env => env.FeatureFlag)
            .Where(env => env.Environment == environment)
            .Select(env => new FlagResponseDTO
            {
                Id = env.FeatureFlag!.Id,
                Key = env.FeatureFlag.Key,
                Name = env.FeatureFlag.Name,
                Description = env.FeatureFlag.Description,
                Environments = env.FeatureFlag.Environments.Select(e => new EnvironmentConfigResponseDTO
                {
                    Environment = e.Environment,
                    Enabled = e.Enabled,
                    PercentageRollout = e.PercentageRollout
                }).ToList()
            })
            .ToList();
    }

    public EnvironmentConfigResponseDTO? GetEnvironmentConfiguration(string flagKey, EnvironmentType environment)
    {
    var normalizedKey = NormalizeKey(flagKey);
    var flag = _dbContext.FeatureFlags
        .AsNoTracking()
        .FirstOrDefault(flag => flag.Key == normalizedKey);

    if (flag is null)
    {
        return null;
    }

    var environmentConfig = _dbContext.FeatureFlagEnvironments
        .AsNoTracking()
        .FirstOrDefault(env =>
            env.FeatureFlagId == flag.Id &&
            env.Environment == environment);

    if (environmentConfig is null)
    {
        return null;
    }

    return MapToEnvironmentResponse(environmentConfig);
    }

    public async Task<EnvironmentConfigResponseDTO?> CreateEnvironmentConfiguration(string flagKey, CreateEnvironmentDTO request)
    {
        var normalizedKey = NormalizeKey(flagKey);
        var flag = _dbContext.FeatureFlags.FirstOrDefault(flag => flag.Key == normalizedKey);

        if (flag is null)
        {
            return null;
        }

        var existingConfig = _dbContext.FeatureFlagEnvironments
            .FirstOrDefault(env =>
                env.FeatureFlagId == flag.Id &&
                env.Environment == request.Environment);

        if (existingConfig != null)
        {
            throw new InvalidOperationException($"Configuration for environment '{request.Environment}' already exists for flag '{flagKey}'.");
        }

        var newConfig = new FeatureFlagEnvironment
        {
            Id = Guid.NewGuid(),
            FeatureFlagId = flag.Id,
            Environment = request.Environment,
            Enabled = request.Enabled,
            PercentageRollout = request.PercentageRollout
        };

        _dbContext.FeatureFlagEnvironments.Add(newConfig);
        _dbContext.SaveChanges();

        await _cache.InvalidateEnvironmentListAsync(request.Environment);

        return MapToEnvironmentResponse(newConfig);
    }

    public async Task<EnvironmentConfigResponseDTO?> UpdateEnvironmentConfiguration(string flagKey, EnvironmentType environment, UpdateEnvironmentDTO request)
    {
        var normalizedKey = NormalizeKey(flagKey);

        var existingConfig = _dbContext.FeatureFlagEnvironments
            .FirstOrDefault(env =>
                env.FeatureFlag!.Key == normalizedKey &&
                env.Environment == environment);

        if (existingConfig == null)
        {
            return null;
        }

        if (request.Enabled.HasValue)
        {
            existingConfig.Enabled = request.Enabled.Value;
        }

        if (request.PercentageRollout.HasValue)
        {
            existingConfig.PercentageRollout = Math.Clamp(request.PercentageRollout.Value, 0, 100);
        }

        _dbContext.SaveChanges();

        await _cache.InvalidateEnvironmentConfigAsync(flagKey, environment);
        await _cache.InvalidateEnvironmentListAsync(environment);
        await _cache.InvalidateFeatureFlagAsync(flagKey);
        
        return MapToEnvironmentResponse(existingConfig);
    }

    public async Task<bool> DeleteEnvironmentConfiguration(string flagKey, EnvironmentType environment)
    {
        var normalizedKey = NormalizeKey(flagKey);

        var existingConfig = _dbContext.FeatureFlagEnvironments
            .FirstOrDefault(env =>
                env.FeatureFlag!.Key == normalizedKey &&
                env.Environment == environment);

        if (existingConfig == null)
        {
            return false;
        }

        _dbContext.FeatureFlagEnvironments.Remove(existingConfig);
        _dbContext.SaveChanges();

        await _cache.InvalidateEnvironmentConfigAsync(flagKey, environment);
        await _cache.InvalidateEnvironmentListAsync(environment);
        await _cache.InvalidateFeatureFlagAsync(flagKey);

        return true;
    }
}
