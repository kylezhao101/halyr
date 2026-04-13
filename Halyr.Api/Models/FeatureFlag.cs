namespace Halyr.Api.Models;

public class FeatureFlag
{
    public Guid Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public ICollection<FeatureFlagEnvironment> Environments { get; set; } = [];
}