using Merlin.Helpers;
using Merlin.Models;
using Microsoft.AspNetCore.Mvc;

namespace Merlin.Controllers
{
    /// <summary>
    /// This provides standalone Python evaluation services based on local system python
    /// </summary>
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
        public IEnumerable<NodeDefinition> GetTopLevelModules()
        {
            throw new NotImplementedException();
        }
        public IEnumerable<NodeDefinition> GetSubmodules(string module)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<NodeDefinition> GetTypesInModule(string module)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<NodeDefinition> GetTopLevelMethodsInModule(string module)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<NodeDefinition> GetMethodsForType(string type)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<NodeDefinition> GetMethodAttributes(string method)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Attribute for type instancing node
        /// </summary>
        public IEnumerable<NodeDefinition> GetTypeAttributes(string method)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Posts
        /// <summary>
        /// Continue or create new execution session
        /// </summary>
        [HttpPost(Name = "Execute")]
        public async Task<string> ExecuteSesion([FromQuery]string session)
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
        /// <summary>
        /// Execute and get result in a throw-away session/standalone invokation
        /// </summary>
        public async Task<string> ExecuteOnce()
        {
            throw new NotImplementedException();
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
