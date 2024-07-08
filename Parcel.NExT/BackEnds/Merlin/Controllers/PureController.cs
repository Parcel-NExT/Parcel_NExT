using Merlin.Helpers;
using Merlin.Models;
using Microsoft.AspNetCore.Mvc;

namespace Merlin.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PureController : ControllerBase
    {
        private readonly ILogger<PureController> _logger;

        public PureController(ILogger<PureController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetPureTargets")]
        public IEnumerable<NodeDefinition> Get()
        {
            return NodeDefinitions.Samples;
        }
    }
}
