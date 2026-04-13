using Halyr.Api.DTOs;
using Halyr.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Halyr.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EvaluationController : ControllerBase
{
    private readonly IFeatureFlagEvaluationService _featureEvaluationService;

    public EvaluationController(IFeatureFlagEvaluationService featureEvaluationService)
    {
        _featureEvaluationService = featureEvaluationService;
    }

    [HttpPost]
    public ActionResult<EvaluateFlagResponse> Evaluate([FromBody] EvaluateFlagRequest request)
    {
        var response = _featureEvaluationService.Evaluate(request);
        return Ok(response);
    }
}