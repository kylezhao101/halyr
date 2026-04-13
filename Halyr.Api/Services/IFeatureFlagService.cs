
using Halyr.Api.Models;
using Halyr.Api.Enums;


namespace Halyr.Api.Services;

public interface IFeatureFlagService
{
    IEnumerable<FeatureFlag> GetAll();
    FeatureFlag? GetByKey(string key);
    FeatureFlagEnvironment? GetEnvironmentConfiguration(string flagKey, EnvironmentType environment);
}