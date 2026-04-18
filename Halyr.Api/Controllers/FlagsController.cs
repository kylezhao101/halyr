using Halyr.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Halyr.Api.DTOs;

namespace Halyr.Api.Controllers

{
    [ApiController]
    [Route("api/[controller]")]
    public class FlagsController : ControllerBase
    {
        private readonly IFeatureFlagService _featureFlagService;

        public FlagsController(IFeatureFlagService featureFlagService)
        {
            _featureFlagService = featureFlagService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var flags = _featureFlagService.GetAll();
            return Ok(flags);
        }

        [HttpGet("{key}")]
        public IActionResult GetByKey(string key)
        {
            var flag = _featureFlagService.GetByKey(key);

            if (flag == null)
            {
                return NotFound();
            }

            return Ok(flag);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateFlagRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdFlag = _featureFlagService.Create(request);
                return CreatedAtAction(nameof(GetByKey), new { key = createdFlag.Key }, createdFlag);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPatch("{key}")]
        public IActionResult Update(string key, [FromBody] UpdateFlagRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedFlag = _featureFlagService.Update(key, request);

            if (updatedFlag == null)
            {
                return NotFound();
            }

            return Ok(updatedFlag);
        }

        [HttpDelete("{key}")]
        public IActionResult Delete(string key)
        {
            var deleted = _featureFlagService.Delete(key);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

        
    }
}