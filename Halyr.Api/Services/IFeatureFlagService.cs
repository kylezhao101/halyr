
using Halyr.Api.Models;
using Halyr.Api.Enums;
using Halyr.Api.DTOs;


namespace Halyr.Api.Services;

public interface IFeatureFlagService
{
    IEnumerable<FlagResponseDTO> GetAll();
    FlagResponseDTO? GetByKey(string key);

    FlagResponseDTO Create(CreateFlagRequestDTO request);
    Task<FlagResponseDTO?> Update(string key, UpdateFlagRequestDTO request);
    Task<bool> Delete(string key);

    IEnumerable<FlagResponseDTO> GetByEnvironment(EnvironmentType environment);
    EnvironmentConfigResponseDTO? GetEnvironmentConfiguration(string flagKey, EnvironmentType environment);
    Task<EnvironmentConfigResponseDTO?> CreateEnvironmentConfiguration(string flagKey, CreateEnvironmentDTO request);
    Task<EnvironmentConfigResponseDTO?> UpdateEnvironmentConfiguration(string flagKey, EnvironmentType environment, UpdateEnvironmentDTO request);
    Task<bool> DeleteEnvironmentConfiguration(string flagKey, EnvironmentType environment);
}