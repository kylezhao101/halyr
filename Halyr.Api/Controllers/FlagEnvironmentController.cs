using Halyr.Api.DTOs;
using Halyr.Api.Enums;
using Halyr.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Halyr.Api.Controllers;

[ApiController]
[Route("api/flags/{flagKey}/environments")]
public class FlagEnvironmentController : ControllerBase
{
    private readonly IFeatureFlagService _featureFlagService;

    public FlagEnvironmentController(IFeatureFlagService featureFlagService)
    {
        _featureFlagService = featureFlagService;
    }

    [HttpGet("{environment}")]
    public IActionResult GetEnvironmentConfiguration(string flagKey, EnvironmentType environment)
    {

        var config = _featureFlagService.GetEnvironmentConfiguration(flagKey, environment);

        if (config == null)
        {
            return NotFound();
        }

        return Ok(config);
    }

    [HttpPost]
    public IActionResult CreateEnvironmentConfiguration(string flagKey, [FromBody] CreateEnvironmentDTO request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var createdConfig = _featureFlagService.CreateEnvironmentConfiguration(flagKey, request);
            if (createdConfig == null)
            {
                return NotFound();
            }
            return CreatedAtAction(nameof(GetEnvironmentConfiguration), new { flagKey, environment = createdConfig.Environment.ToString() }, createdConfig);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPatch("{environment}")]
    public IActionResult UpdateEnvironmentConfiguration(string flagKey, EnvironmentType environment, [FromBody] UpdateEnvironmentDTO request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var updatedConfig = _featureFlagService.UpdateEnvironmentConfiguration(flagKey, environment, request);

        if (updatedConfig == null)
        {
            return NotFound();
        }

        return Ok(updatedConfig);
    }

    [HttpDelete("{environment}")]
    public IActionResult DeleteEnvironmentConfiguration(string flagKey, EnvironmentType environment)
    {
        var success = _featureFlagService.DeleteEnvironmentConfiguration(flagKey, environment);

        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }
}