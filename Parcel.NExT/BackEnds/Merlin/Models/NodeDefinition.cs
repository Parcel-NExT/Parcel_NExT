namespace Merlin.Models
{
    public record NodeAttribute(string Name, string Type);
    public class NodeDefinition
    {
        public string TargetPath { get; set; }
        public string Namespace { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }

        public string Function { get; set; }

        public NodeAttribute[] Attributes { get; set; }
    }
}
