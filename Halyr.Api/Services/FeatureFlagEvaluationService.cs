using Halyr.Api.DTOs;
using Halyr.Api.Helpers;

namespace Halyr.Api.Services;

public class FeatureFlagEvaluationService : IFeatureFlagEvaluationService
{
    private readonly IFeatureFlagService _featureFlagService;

    public FeatureFlagEvaluationService(IFeatureFlagService featureFlagService)
    {
        _featureFlagService = featureFlagService;
    }

    public EvaluateFlagResponse Evaluate(EvaluateFlagRequest request)
    {
        var flag = _featureFlagService.GetByKey(request.FlagKey);

        if (flag is null)
        {
            return new EvaluateFlagResponse
            {
                FlagKey = request.FlagKey,
                UserId = request.UserId,
                Enabled = false,
                Bucket = -1,
                PercentageRollout = 0 
            };
        }

        var config = _featureFlagService.GetEnvironmentConfiguration(request.FlagKey, request.Environment);

        if (config is null)
        {
            return new EvaluateFlagResponse
            {
                FlagKey = request.FlagKey,
                UserId = request.UserId,
                Environment = request.Environment,
                Enabled = false,
                Bucket = -1,
                PercentageRollout = 0
            };
        }

        if (!config.Enabled)
        {
            return new EvaluateFlagResponse
            {
                FlagKey = flag.Key,
                UserId = request.UserId,
                Environment = config.Environment,
                Enabled = false,
                Bucket = -1,
                PercentageRollout = config.PercentageRollout
            };
        }

        var rollout = Math.Clamp(config.PercentageRollout, 0, 100);
        var bucket = HashingHelper.GetBucket(request.FlagKey, request.UserId);
        var enabled = bucket < rollout;

        return new EvaluateFlagResponse
        {
            FlagKey = flag.Key,
            UserId = request.UserId,
            Environment = config.Environment,
            Enabled = enabled,
            Bucket = bucket,
            PercentageRollout = rollout
        };
    }
}