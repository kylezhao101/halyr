using Halyr.Api.Services;
using Microsoft.AspNetCore.Mvc;

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

            if (flags == null)
            {
                return NotFound();
            }

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
    }
}