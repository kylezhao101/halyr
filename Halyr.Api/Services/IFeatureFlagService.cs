
using Halyr.Api.Models;
using Halyr.Api.Enums;
using Halyr.Api.DTOs;


namespace Halyr.Api.Services;

public interface IFeatureFlagService
{
    IEnumerable<FlagResponseDTO> GetAll();
    FlagResponseDTO? GetByKey(string key);
    FeatureFlagEnvironment? GetEnvironmentConfiguration(string flagKey, EnvironmentType environment);
}