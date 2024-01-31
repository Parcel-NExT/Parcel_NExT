using Merlin.Helpers;
using Merlin.Models;
using Microsoft.AspNetCore.Mvc;

namespace Merlin.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PythonController : ControllerBase
    {
        #region Construction
        private readonly ILogger<PythonController> _logger;
        public PythonController(ILogger<PythonController> logger)
        {
            _logger = logger;
        }
        #endregion

        #region Data
        public static Dictionary<string, InteractiveProcess> Sessions = [];
        #endregion

        #region Gets
        [HttpGet(Name = "GetPythonTargets")]
        public IEnumerable<NodeDefinition> Get()
        {
            return NodeDefinitions.Samples;
        }
        #endregion

        #region Posts
        [HttpPost(Name = "Execute")]
        public async Task<string> Execute([FromQuery]string session)
        {
            string scripts = await Request.Body.ReadAsStringAsync();

            if (!Sessions.ContainsKey(session))
            {
                var intermediaryPath = DependencyLocator.GetIntermediaryPath();
                var newSession = new InteractiveProcess(Path.Combine(intermediaryPath, "InteractivePython.exe"));
                newSession.Start();
                Sessions[session] = newSession;
            }

            InteractiveProcess process = Sessions[session];
            return process.SendCommand(scripts);
        }
        #endregion
    }

    public static class RequestExtensions
    {
        public static async Task<string> ReadAsStringAsync(this Stream requestBody, bool leaveOpen = false)
        {
            using StreamReader reader = new(requestBody, leaveOpen: leaveOpen);
            var bodyAsString = await reader.ReadToEndAsync();
            return bodyAsString;
        }
    }
}
