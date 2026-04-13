using Halyr.Api.Enums;

namespace Halyr.Api.Models;

public class FeatureFlagEnvironment
{
    public Guid Id { get; set; }
    public Guid FeatureFlagId { get; set; }
    public EnvironmentType Environment { get; set; } = EnvironmentType.Development;
    public bool Enabled { get; set; }
    public int PercentageRollout { get; set; } = 100;
}