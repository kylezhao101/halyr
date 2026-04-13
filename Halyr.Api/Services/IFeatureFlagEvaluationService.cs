using Halyr.Api.DTOs;

namespace Halyr.Api.Services;

public interface IFeatureFlagEvaluationService
{
    EvaluateFlagResponse Evaluate(EvaluateFlagRequest request);
}