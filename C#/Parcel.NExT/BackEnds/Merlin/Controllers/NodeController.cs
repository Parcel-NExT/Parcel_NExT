using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Merlin.Controllers
{
    /// <summary>
    /// Pertaining to node data and runtime execution
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class NodeController : ControllerBase
    {
        // GET: api/<NodeController>
        [HttpGet]
        public IEnumerable<string> GetNodeAttributeEnumOptions()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet]
        public string GetNodePreview()
        {
            throw new NotImplementedException();
        }
        [HttpGet]
        public string GetNodePreviewForPlotting()
        {
            throw new NotImplementedException();
        }

        // GET api/<NodeController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<NodeController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<NodeController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<NodeController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
