using Merlin.Helpers;
using Merlin.Models;
using Microsoft.AspNetCore.Mvc;

namespace Merlin.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CSharpController : ControllerBase
    {
        private readonly ILogger<CSharpController> _logger;

        public CSharpController(ILogger<CSharpController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetCSharpTargets")]
        public IEnumerable<NodeDefinition> Get()
        {
            return NodeDefinitions.Samples;
        }
    }
}
