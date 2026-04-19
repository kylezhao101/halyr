using Halyr.Api.DTOs;

namespace Halyr.Api.Services;

public interface IFeatureFlagEvaluationService
{
    Task<EvaluateFlagResponseDTO> Evaluate(EvaluateFlagRequestDTO request);
}