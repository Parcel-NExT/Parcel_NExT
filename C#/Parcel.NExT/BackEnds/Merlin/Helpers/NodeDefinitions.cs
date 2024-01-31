using Merlin.Models;

namespace Merlin.Helpers
{
    public static class NodeDefinitions
    {
        public static List<NodeDefinition> Samples = [
            new NodeDefinition
            {
                TargetPath = "WriteLine",
                Function = "WriteLine",
                Type = "WriteLine",
                Namespace = "Top",
                Attributes = [
                    new NodeAttribute("Parameter1", "string")
                ],
                Description = "Print line."
            }
        ];
    }
}
