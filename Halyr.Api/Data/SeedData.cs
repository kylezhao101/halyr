using Halyr.Api.Enums;
using Halyr.Api.Models;

namespace Halyr.Api.Data;

public static class SeedData
{
    public static void Initialize(AppDbContext db)
    {
        if (db.FeatureFlags.Any())
        {
            return;
        }

        var newDashboardFlagId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var betaUploadFlagId = Guid.Parse("22222222-2222-2222-2222-222222222222");

        var flags = new List<FeatureFlag>
        {
            new()
            {
                Id = newDashboardFlagId,
                Key = "new_dashboard",
                Name = "New Dashboard"
            },
            new()
            {
                Id = betaUploadFlagId,
                Key = "beta_upload_flow",
                Name = "Beta Upload Flow"
            }
        };

        var environments = new List<FeatureFlagEnvironment>
        {
            new()
            {
                Id = Guid.NewGuid(),
                FeatureFlagId = newDashboardFlagId,
                Environment = EnvironmentType.Development,
                Enabled = true,
                PercentageRollout = 100
            },
            new()
            {
                Id = Guid.NewGuid(),
                FeatureFlagId = newDashboardFlagId,
                Environment = EnvironmentType.Staging,
                Enabled = true,
                PercentageRollout = 100
            },
            new()
            {
                Id = Guid.NewGuid(),
                FeatureFlagId = newDashboardFlagId,
                Environment = EnvironmentType.Production,
                Enabled = true,
                PercentageRollout = 25
            },
            new()
            {
                Id = Guid.NewGuid(),
                FeatureFlagId = betaUploadFlagId,
                Environment = EnvironmentType.Production,
                Enabled = false,
                PercentageRollout = 100
            }
        };

        db.FeatureFlags.AddRange(flags);
        db.FeatureFlagEnvironments.AddRange(environments);
        db.SaveChanges();
    }
}