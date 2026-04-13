using Halyr.Api.Models;
using Halyr.Api.Enums;

namespace Halyr.Api.Services;


public class FeatureFlagService : IFeatureFlagService
{
    private readonly List<FeatureFlag> _flags =
    [
        new FeatureFlag
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Key = "new_dashboard",
            Name = "New Dashboard"
        },
        new FeatureFlag
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            Key = "beta_upload_flow",
            Name = "Beta Upload Flow"
        }
    ];

    private readonly List<FeatureFlagEnvironment> _environments =
    [
        new FeatureFlagEnvironment
        {
            Id = Guid.NewGuid(),
            FeatureFlagId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Environment = EnvironmentType.Development,
            Enabled = true,
            PercentageRollout = 100
        },
        new FeatureFlagEnvironment
        {
            Id = Guid.NewGuid(),
            FeatureFlagId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Environment = EnvironmentType.Staging,
            Enabled = true,
            PercentageRollout = 100
        },
        new FeatureFlagEnvironment
        {
            Id = Guid.NewGuid(),
            FeatureFlagId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Environment = EnvironmentType.Production,
            Enabled = true,
            PercentageRollout = 25
        },
        new FeatureFlagEnvironment
        {
            Id = Guid.NewGuid(),
            FeatureFlagId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            Environment = EnvironmentType.Production,
            Enabled = false,
            PercentageRollout = 100
        }
    ];

    public IEnumerable<FeatureFlag> GetAll()
    {        
        return _flags;
    }

    public FeatureFlag? GetByKey(string key)
    {
        return _flags.FirstOrDefault(flag =>
            flag.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
    }

    public FeatureFlagEnvironment? GetEnvironmentConfiguration(string flagKey, EnvironmentType environment)
    {
        var flag = GetByKey(flagKey);

        if (flag is null)
        {
            return null;
        }

        return _environments.FirstOrDefault(env =>
            env.FeatureFlagId == flag.Id &&
            env.Environment == environment);
    }
}
