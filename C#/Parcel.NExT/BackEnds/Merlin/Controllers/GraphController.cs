using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Merlin.Controllers
{
    /// <summary>
    /// Atomic operational graph editing capabilities (used by some frontends).
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class GraphController : ControllerBase
    {
        // CreateGraph
        // RenameGraph
        // DeleteGraph
        // AddNode
        // MoveNode
        // ModifyNode
        // DeleteNode
    }
}
