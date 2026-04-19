using Halyr.Api.DTOs;
using Halyr.Api.Helpers;
using Halyr.Api.Common.Cache;

namespace Halyr.Api.Services;

public class FeatureFlagEvaluationService : IFeatureFlagEvaluationService
{
    private readonly IFeatureFlagService _featureFlagService;
    private readonly IFeatureFlagCache _cache;

    public FeatureFlagEvaluationService(IFeatureFlagService featureFlagService, IFeatureFlagCache cache)
    {
        _featureFlagService = featureFlagService;
        _cache = cache;
    }

    public async Task<EvaluateFlagResponseDTO> Evaluate(EvaluateFlagRequestDTO request)
    {
        var config = _cache.GetEnvironmentConfigAsync(request.FlagKey, request.Environment).Result;

        if (config is null)
        {
            config = _featureFlagService.GetEnvironmentConfiguration(request.FlagKey, request.Environment);

            if (config is null)
            {
                return new EvaluateFlagResponseDTO
                {
                    FlagKey = request.FlagKey,
                    UserId = request.UserId,
                    Environment = request.Environment,
                    Enabled = false,
                    Bucket = -1,
                    PercentageRollout = 0
                };
            }

            await _cache.SetEnvironmentConfigAsync(
                request.FlagKey,
                request.Environment,
                config,
                CacheTtls.EnvironmentConfig
            );
        }

        var rollout = Math.Clamp(config.PercentageRollout, 0, 100);
        var bucket = HashingHelper.GetBucket(request.FlagKey, request.UserId);
        var enabled = bucket < rollout;

        return new EvaluateFlagResponseDTO
        {
            FlagKey = request.FlagKey,
            UserId = request.UserId,
            Environment = config.Environment,
            Enabled = enabled,
            Bucket = bucket,
            PercentageRollout = rollout
        };
    }
}