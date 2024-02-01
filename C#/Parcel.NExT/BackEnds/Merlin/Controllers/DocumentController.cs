using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Merlin.Controllers
{
    /// <summary>
    /// Operating system isolation layer for file operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {        
        [HttpGet("{id}")]
        public string OpenDocument(string path)
        {
            return "value";
        }
        [HttpPost]
        public void SaveDocument(string path, [FromBody] string value)
        {
        }
        [HttpDelete("{id}")]
        public void DeleteDocument(int id)
        {
        }
    }
}
