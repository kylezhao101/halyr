using Halyr.Api.Models;
using Halyr.Api.Enums;
using Halyr.Api.Data;
using Microsoft.EntityFrameworkCore;
using Halyr.Api.DTOs;

namespace Halyr.Api.Services;


public class FeatureFlagService : IFeatureFlagService
{
    private readonly AppDbContext _dbContext;

    public FeatureFlagService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
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
                Environments = flag.Environments.Select(env => new FlagEnvironmentDTO
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
        return _dbContext.FeatureFlags
            .AsNoTracking()
            .Include(flag => flag.Environments)
            .Where(flag => flag.Key == key)
            .Select(flag => new FlagResponseDTO
            {   
                Id = flag.Id,
                Key = flag.Key,
                Name = flag.Name,
                Environments = flag.Environments.Select(env => new FlagEnvironmentDTO
                {
                    Environment = env.Environment,
                    Enabled = env.Enabled,
                    PercentageRollout = env.PercentageRollout
                }).ToList()
            })
            .FirstOrDefault();
    }

    public FeatureFlagEnvironment? GetEnvironmentConfiguration(string flagKey, EnvironmentType environment)
    {
    var flag = _dbContext.FeatureFlags
        .AsNoTracking()
        .FirstOrDefault(flag => flag.Key == flagKey);

    if (flag is null)
    {
        return null;
    }

    return _dbContext.FeatureFlagEnvironments
        .AsNoTracking()
        .FirstOrDefault(env =>
            env.FeatureFlagId == flag.Id &&
            env.Environment == environment);
    }
}
